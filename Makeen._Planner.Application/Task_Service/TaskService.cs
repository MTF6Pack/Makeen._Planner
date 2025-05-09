using Application.Contracts.Tasks;
using Application.Contracts.Tasks.Commands;
using Domain;
using Domain.Events;
using Domain.TaskEnums;
using Infrastructure.Date_and_Time;
using Infrastructure.Exceptions;
using Infrastructure.SignalR;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository;
using Persistence.Repository.Interface;
using Task = System.Threading.Tasks.Task;

namespace Application.Task_Service
{
    public class TaskService(ITaskRepository repository, IUnitOfWork unitOfWork, IMediator mediator, NotificationSenderHandler notificationSender) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly NotificationSenderHandler _notificationSender = notificationSender;

        public async Task<bool> AddTask(AddTaskCommand command, Guid userId)
        {
            ValidateTaskTimings(command);
            if (command.GroupId.HasValue)
            { if (!await _repository.StraitAccess.Set<Group>().AnyAsync(g => g.Id == command.GroupId)) { throw new NotFoundException("Group"); } }
            bool? groupType = null;
            if (command.GroupId.HasValue) groupType = await _repository.StraitAccess.Set<Group>().AsNoTracking().Where(g => g.Id == command.GroupId)
            .Select(g => (bool?)g.Grouptype).FirstOrDefaultAsync();

            bool isGroupPublic = groupType == true;
            bool hasGroupId = command.GroupId.HasValue;
            bool hasReceiver = command.ReceiverId.HasValue;

            bool isAdminForAllUsers = isGroupPublic && hasGroupId && !hasReceiver;
            bool isAdminForSingleUser = isGroupPublic && hasGroupId && hasReceiver;
            bool isUserForUser = !isGroupPublic && !hasGroupId && hasReceiver;
            bool isUserSelfList = !isGroupPublic && hasGroupId && !hasReceiver;
            bool isUserSelf = !isGroupPublic && !hasGroupId && !hasReceiver;

            Guid? senderId = !isUserSelf ? userId : null;

            if (isAdminForAllUsers) await HandleAdminForAllUsers(command, userId, senderId);
            else if (isAdminForSingleUser) await HandleAdminForSingleUser(command, userId, senderId);
            else if (isUserForUser) await HandleUserForUser(command, senderId);
            else if (isUserSelf || isUserSelfList) return await HandleUserSelf(command, userId);
            else throw new BadRequestException("There are 5 types of add-task and your request matches none of them");

            return false;
        }
        private static void ValidateTaskTimings(AddTaskCommand command)
        {
            if (command.DeadLine < DateTime.Now || command.StartTime < DateTime.Now) throw new BadRequestException("Deadline nor Start time cannot be in the past");
            if (command.DeadLine < command.StartTime) throw new BadRequestException("Deadline cannot be before Starttime");
        }
        private async Task HandleAdminForAllUsers(AddTaskCommand command, Guid userId, Guid? senderId)
        {
            bool isAdmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(m => m.GroupId == command.GroupId && m.UserId == userId && m.IsAdmin);
            if (!isAdmin) throw new UnauthorizedException("User is not an admin in this group");

            var groupMembers = await _repository.StraitAccess.Set<GroupMembership>().Include(m => m.User).Where(m => m.GroupId == command.GroupId && m.IsAdmin == false).ToListAsync();
            foreach (var member in groupMembers)
            {
                var taskForMember = command.ToModel(senderId);
                member.User.Tasks.Add(taskForMember);
                _repository.StraitAccess.Set<Domain.Task>().Add(taskForMember);
                await SendTaskNotif(taskForMember, NotificationType.Order, "تسک جدیدی از طرف ادمین برای شما تعریف شد", member.UserId);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task<bool> HandleAdminForSingleUser(AddTaskCommand command, Guid userId, Guid? senderId)
        {
            var isadmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(m => m.GroupId == command.GroupId && m.UserId == userId && m.IsAdmin);
            if (!isadmin) throw new UnauthorizedException("User is not an admin in this group");

            var targetUser = await _repository.StraitAccess.Set<User>().Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == command.ReceiverId) ?? throw new NotFoundException("User");
            var adminTask = command.ToModel(senderId);
            targetUser.Tasks.Add(adminTask);
            _repository.StraitAccess.Set<Domain.Task>().Add(adminTask);
            await SendTaskNotif(adminTask, NotificationType.Order, "تسک جدیدی از طرف ادمین برای شما تعریف شد", (Guid)command.ReceiverId!);
            await _unitOfWork.SaveChangesAsync();
            return await CheckConflict(command, userId, adminTask.Id);
        }
        private async Task HandleUserForUser(AddTaskCommand command, Guid? senderId)
        {
            var targetUser = await _repository.StraitAccess.Set<User>().Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == command.ReceiverId) ?? throw new NotFoundException("User");
            var taskForUser = command.ToModel(senderId);
            // Notify the receiver; assume SendTaskNotif handles notification and task assignment as needed
            await SendTaskNotif(taskForUser, NotificationType.Request, "درخواست جدید", (Guid)command.ReceiverId!);
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task<bool> HandleUserSelf(AddTaskCommand command, Guid userId, Guid? senderId = null)
        {
            var targetUser = await _repository.StraitAccess.Set<User>().Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == userId);
            var task = command.ToModel(senderId);
            targetUser?.Tasks.Add(task);
            _repository.StraitAccess.Set<Domain.Task>().Add(task);
            await _unitOfWork.SaveChangesAsync();
            return await CheckConflict(command, userId, task.Id);
        }
        private async Task<bool> CheckConflict(AddTaskCommand command, Guid userId, Guid taskid)
        {
            return await _repository.StraitAccess.Set<Domain.Task>().Where(t => t.User!.Id == userId && t.Id != taskid).AnyAsync(t =>
            t.StartTime.Year == command.StartTime.Year &&
            t.StartTime.Month == command.StartTime.Month &&
            t.StartTime.Day == command.StartTime.Day &&
            t.StartTime.Hour == command.StartTime.Hour &&
            t.StartTime.Minute == command.StartTime.Minute);
        }

        public async Task<List<Domain.Task>> GetTheUserOrGroupTasksByCalander(DateTime? date, Guid userId, Guid? groupId, bool isGroupTask)
        {
            // Convert the incoming (possibly Persian) date to a Gregorian range for the day
            var theDate = date ?? DateTime.Now.Date;
            DateTime1 greg = DateHelper.ConvertPersianToGregorian(theDate, false);
            var startOfDay = greg;
            var endOfDay = greg.AddDays(1);

            // Base query: always include Instances
            var query = _repository.StraitAccess
                .Set<Domain.Task>()
                .Include(t => t.Instances)
                .AsQueryable();

            // Apply the Pending + date‐range filter
            query = query.Where(t => (
                t.StartTime >= startOfDay &&
                t.StartTime < endOfDay &&
                t.Status == Domain.TaskEnums.Status.Pending) ||
    (
      // recurring tasks by their next occurrence
      (t.Repeat != Repeat.None &&
       t.NextInstance >= startOfDay &&
       t.NextInstance < endOfDay)
    // you can choose to show even if Status == Done
    ));

            // Now branch on whether it’s a personal vs. group task
            if (groupId.HasValue)
            {
                query = query.Where(t =>
                    t.GroupId == groupId.Value &&
                    t.User!.Id == userId
                );
            }
            else if (isGroupTask)
            {
                query = query.Where(t =>
                    t.IsInGroup == true &&
                    t.User!.Id == userId
                );
            }
            else
            {
                query = query.Where(t =>
                    t.IsInGroup == false &&
                    t.User!.Id == userId
                );
            }

            // Finally, sort and materialize
            return await query
                .OrderByDescending(t => t.CreationTime)
                .ToListAsync();
        }

        public async Task<List<Domain.Task>> GetAdminSentTasks(DateTime? date, Guid userid, Guid groupid)
        {
            var thedate = date ?? DateTime.Now.Date;
            DateTime1 gregorianDate = DateHelper.ConvertPersianToGregorian(thedate, false);
            var startOfDay = gregorianDate;
            var endOfDay = gregorianDate.AddDays(1);
            return await _repository.StraitAccess.Set<Domain.Task>().Where(t => t.GroupId == groupid && t.SenderId == userid && t.StartTime >= startOfDay && t.StartTime < endOfDay).ToListAsync();
        }
        public async Task RemoveTask(Guid taskid)
        {
            Domain.Task? thetask = await _repository.GetByIdAsync(taskid);
            if (thetask == null) throw new NotFoundException(nameof(thetask));
            _repository.Delete(thetask);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateTask(Guid taskId, UpdateTaskCommand command)
        {
            // Fetch the task using taskId
            Domain.Task? thetask = await _repository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task");

            // Directly use PriorityCategory if it's non-nullable
            thetask.UpdateTask(command.Name, command.DeadLine, command.PriorityCategory, command.StartTime, command.Repeat, command.Alarm, command.Description);

            // Commit changes using Unit of Work
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Done(Guid taskid)
        {
            var task = await _repository.StraitAccess.Set<Domain.Task>().Include(t => t.User).FirstOrDefaultAsync(t => t.Id == taskid) ?? throw new NotFoundException("Task");
            if (task.Status == Domain.TaskEnums.Status.Done) throw new BadRequestException("Task is already done");
            task.Done();
            if (task.Done()) await SendTaskNotif(task, NotificationType.System, $"{task.Repeat!.Value} Task '{task.Name}' repeat is set.", task.User!.Id);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Done(List<Guid>? tasksid, DateTime? date)
        {
            if (tasksid is { Count: > 0 } && date is null)
            {
                var tasks = await _repository.StraitAccess.Set<Domain.Task>().Include(t => t.User).Where(t => tasksid.Contains(t.Id) && t.Status == Domain.TaskEnums.Status.Pending).ToListAsync();

                foreach (var task in tasks)
                {
                    task.Done();
                    if (task.Done()) await SendTaskNotif(task, NotificationType.System, $"{task.Repeat!.Value} Task '{task.Name}' repeat is set.", task.User!.Id);
                }
                await _unitOfWork.SaveChangesAsync();
                return;
            }

            if (tasksid is null && date is not null)
            {
                var tasks = await _repository.StraitAccess.Set<Domain.Task>().Include(t => t.User).Where(t => t.StartTime.Date == date && t.Status == Domain.TaskEnums.Status.Pending).ToListAsync();
                foreach (var task in tasks)
                {
                    task.Done();
                    if (task.Done()) await SendTaskNotif(task, NotificationType.System, $"{task.Repeat!.Value} Task '{task.Name}' repeat is set.", task.User!.Id);
                }
                await _unitOfWork.SaveChangesAsync();
                return;
            }
            throw new BadRequestException();
        }
        private async Task SendTaskNotif(Domain.Task task, NotificationType notificationType, string message, Guid receiverId)
        {
            // Create the notification
            Notification notification = new(task, message, notificationType, task.SenderId, receiverId);

            // Retrieve the user and include their notifications (important for adding the new notification to their list)
            var user = await _repository.StraitAccess.Set<User>().Include(u => u.Notifications).FirstOrDefaultAsync(u => u.Id == receiverId);
            if (user == null)
            {
                return;
            }

            // Add the new notification to the user's notifications list
            user!.Notifications!.Add(notification);

            // Add the notification to the database
            _repository.StraitAccess.Set<Notification>().Add(notification);

            // Publish the TaskReminderEvent
            await _mediator.Publish(new TaskReminderEvent(receiverId, notification));

            // Attempt to deliver the notification or queue it if the user is not connected
            await _notificationSender.HandleUndeliveredNotifications(CancellationToken.None);
        }
        //public TaskService()
        //{

        //}
    }
}