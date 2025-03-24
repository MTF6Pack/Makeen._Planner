using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository;
using Persistence.Repository.Interface;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Task = System.Threading.Tasks.Task;

namespace Makeen._Planner.Task_Service
{
    public class TaskService(ITaskRepository repository, IUnitOfWork unitOfWork) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> AddTask(AddTaskCommand command, Guid userId)
        {
            ValidateTaskTimings(command);
            bool? groupType = null;
            if (command.GroupId.HasValue) groupType = await _repository.StraitAccess.Set<Group>().AsNoTracking().Where(g => g.Id == command.GroupId)
            .Select(g => (bool?)g.Grouptype).FirstOrDefaultAsync();

            bool isGroupPublic = groupType == true;
            bool hasGroupId = command.GroupId.HasValue;
            bool hasReceiver = command.ReceiverId.HasValue;

            bool isAdminForAllUsers = isGroupPublic && hasGroupId && !hasReceiver;
            bool isAdminForSingleUser = isGroupPublic && hasGroupId && hasReceiver;
            bool isUserForUser = isGroupPublic && !hasGroupId && hasReceiver;
            bool isUserSelfList = !isGroupPublic && hasGroupId && !hasReceiver;
            bool isUserSelf = !isGroupPublic && !hasGroupId && !hasReceiver;

            Guid? senderId = !isUserSelf ? userId : null;

            if (isAdminForAllUsers)
            {
                await HandleAdminForAllUsers(command, userId, senderId);
            }
            else if (isAdminForSingleUser)
            {
                await HandleAdminForSingleUser(command, userId, senderId);
            }
            else if (isUserForUser)
            {
                await HandleUserForUser(command, userId, senderId);
            }
            else if (isUserSelf || isUserSelfList)
            {
                return await HandleUserSelf(command, userId);
            }
            else
            {
                throw new BadRequestException("There are 5 types of add-task and your request matches none of them");
            }

            return false;
        }
        private static void ValidateTaskTimings(AddTaskCommand command)
        {
            if (command.DeadLine < DateTime.Now || command.StartTime < DateTime.Now) throw new BadRequestException("Deadline nor Start time cannot be in the past");
            if (command.DeadLine < command.StartTime) throw new BadRequestException("Deadline cannot be before Starttime");
        }
        private async Task HandleAdminForAllUsers(AddTaskCommand command, Guid userId, Guid? senderId)
        {
            var group = await _repository.StraitAccess.Set<Group>().FindAsync(command.GroupId) ?? throw new NotFoundException("Group");
            bool isAdmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(m => m.GroupId == command.GroupId && m.UserId == userId && m.IsAdmin);
            if (!isAdmin) throw new UnauthorizedException("User is not an admin in this group");

            var groupMembers = await _repository.StraitAccess.Set<GroupMembership>().Include(m => m.User).Where(m => m.GroupId == command.GroupId).ToListAsync();
            foreach (var member in groupMembers)
            {
                if (member.UserId != senderId)
                {
                    var taskForMember = command.ToModel(senderId);
                    member.User.Tasks.Add(taskForMember);
                    _repository.StraitAccess.Set<Domain.Task>().Add(taskForMember);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task<bool> HandleAdminForSingleUser(AddTaskCommand command, Guid userId, Guid? senderId)
        {
            var group = await _repository.StraitAccess.Set<Group>().FindAsync(command.GroupId) ?? throw new NotFoundException("Group");
            var isadmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(m => m.GroupId == command.GroupId && m.UserId == userId && m.IsAdmin);
            if (!isadmin) throw new UnauthorizedException("User is not an admin in this group");

            var targetUser = await _repository.StraitAccess.Set<User>().Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == command.ReceiverId) ?? throw new NotFoundException("User");
            var adminTask = command.ToModel(senderId);
            targetUser.Tasks.Add(adminTask);
            _repository.StraitAccess.Set<Domain.Task>().Add(adminTask);
            await _unitOfWork.SaveChangesAsync();
            return await CheckConflict(command, userId, adminTask.Id);
        }
        private async Task HandleUserForUser(AddTaskCommand command, Guid userId, Guid? senderId)
        {
            var targetUser = await _repository.StraitAccess.Set<User>().Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == command.ReceiverId) ?? throw new NotFoundException("User not found");
            var taskForUser = command.ToModel(senderId);
            // Notify the receiver; assume SendTaskRequestNotif handles notification and task assignment as needed
            await SendTaskRequestNotif(taskForUser, "درخواست جدید", userId);
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

        public async Task<List<Domain.Task>> GetTheUserOrGroupTasksByCalander(DateTime? date, Guid userid, Guid? groupid, bool isGrouptask)
        {
            var thedate = date ?? DateTime.Now.Date;

            IQueryable<Domain.Task> query;

            if (!groupid.HasValue)
            {
                if (!isGrouptask)
                {
                    query = _repository.StraitAccess.Set<User>()
                        .Where(u => u.Id == userid)
                        .SelectMany(u => u.Tasks!.Where(t =>
                            t.StartTime.Date == thedate &&
                            t.IsInGroup == false && t.Status == Domain.TaskEnums.Status.Pending));
                }
                else
                {
                    query = _repository.StraitAccess.Set<Domain.Task>()
                        .Where(t => t.User!.Id == userid && t.IsInGroup == true &&
                            t.StartTime.Date.Date == thedate);
                }
            }
            else
            {
                query = _repository.StraitAccess.Set<Domain.Task>()
                    .Where(t => t.GroupId == groupid &&
                        t.StartTime.Date == thedate && t.Status == Domain.TaskEnums.Status.Pending);
            }

            return await query.OrderByDescending(t => t.CreationTime).ToListAsync();
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
            var task = await _repository.GetByIdAsync(taskid) ?? throw new NotFoundException("Task");
            if (task.Status == Domain.TaskEnums.Status.Done) throw new BadRequestException("Task is already done");
            task.Done();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Done(List<Guid>? tasksid, DateTime? date)
        {
            if (tasksid is { Count: > 0 } && date is null)
            {
                var tasks = await _repository.StraitAccess.Set<Domain.Task>().Where(t => tasksid.Contains(t.Id) && t.Status == Domain.TaskEnums.Status.Pending).ToListAsync();

                foreach (var task in tasks)
                {
                    task.Done();
                }
                await _unitOfWork.SaveChangesAsync();
                return;
            }

            if (tasksid is null && date is not null)
            {
                var tasks = await _repository.StraitAccess.Set<Domain.Task>().Where(t => t.StartTime.Date == date && t.Status == Domain.TaskEnums.Status.Pending).ToListAsync();
                foreach (var task in tasks)
                {
                    task.Done();
                }
                await _unitOfWork.SaveChangesAsync();
                return;
            }
            throw new BadRequestException();
        }
        private async Task SendTaskRequestNotif(Domain.Task task, string message, Guid userid)
        {
            Notification notification = new(task, message, userid);
            var user = await _repository.StraitAccess.Set<User>().FindAsync(userid);
            user!.Notifications!.Add(notification);
            _repository.StraitAccess.Set<Notification>().Add(notification);
        }
        //public async Task<List<Domain.Task.Task>?> GetAllTasks()
        //{
        //    return await _repository.GetAllAsync();
        //}

        //public async Task<List<Domain.Task.Task>> GetObjectByName(string name)
        //{
        //    List<Domain.Task.Task> tasks = await _repository.StraitAccess.Set<Domain.Task.Task>().Where(x => x.Name == name).OrderByDescending(t => t.CreationTime).ToListAsync();
        //    return tasks;
        //}
        //public async Task<List<Domain.Task.Task>> GetAllUserTasks(Guid userid)
        //{
        //    return await _repository.StraitAccess.Set<User>().Where(u => u.Id == userid).SelectMany(u => u.Tasks).OrderByDescending(t => t.CreationTime).ToListAsync();
        //}
    }
}
