using Application.Contracts.Notifications;
using Application.Contracts.Tasks.Commands;
using Domain;
using Infrastructure.Date_and_Time;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Persistence.Repository;
using Task = System.Threading.Tasks.Task;

namespace Application.Notification_Service
{
    public class NotificationService(DataBaseContext dbContext, IUnitOfWork unitofwork) : INotificationService
    {
        private readonly DataBaseContext _dbContext = dbContext;
        private readonly IUnitOfWork _UnitOfWork = unitofwork;

        private object? GetSenderInfo(Guid? senderId, Guid? groupId)
        {
            var userInfo = _dbContext.Users
                    .Where(user => user.Id == senderId)
                    .Select(user => new SenderInfoDto
                    {
                        SenderName = user.Fullname ?? "",
                        SenderUserName = user.UserName ?? "",
                        SenderAvatarUrl = user.AvatarUrl ?? ""
                    })
                    .FirstOrDefault();

            var groupInfo = _dbContext.Groups
                    .Where(group => group.Id == groupId)
                    .Select(group => new SenderInfoDto
                    {
                        GroupName = group.Title ?? ""
                    })
                    .FirstOrDefault();

            return new
            {
                SenderName = userInfo?.SenderName ?? "",
                SenderUserName = userInfo?.SenderUserName ?? "",
                SenderAvatarUrl = userInfo?.SenderAvatarUrl ?? "",
                GroupName = groupInfo?.GroupName ?? ""
            };
        }
        public async Task<object?> GetTheDueTask(Guid userId, Guid notificationId)
        {
            var taskDueData = await _dbContext.Notifications.Include(n => n.Task).ThenInclude(t => t!.User)
                .Where(n => n.Id == notificationId)
                .OrderByDescending(n => n.Task!.CreationTime)
                .Select(n => new
                {
                    n.Id,
                    n.Type,
                    n.Snooze,
                    TaskId = n.Task!.Id,
                    CreationTime = DateHelper.ConvertGregorianToPersian(n.Task.CreationTime, true).persianDate,
                    DeadLine = DateHelper.ConvertGregorianToPersian(n.Task.DeadLine, true).persianDate,
                    n.Task.Name,
                    n.Task.Description,
                    n.Task.PriorityCategory,
                    n.Task.Repeat,
                    n.Message,
                    n.Task.SenderId,
                    n.Task.GroupId,
                    n.Task.Status
                })
                .FirstOrDefaultAsync();

            if (taskDueData == null) return null;

            var senderInfo = GetSenderInfo(taskDueData.SenderId, taskDueData.GroupId);


            return new
            {
                taskDueData.Type,
                taskDueData.Id,
                taskDueData.TaskId,
                taskDueData.CreationTime,
                taskDueData.DeadLine,
                taskDueData.Name,
                taskDueData.Description,
                taskDueData.PriorityCategory,
                taskDueData.Message,

                SenderInfo = senderInfo
            };
        }
        public async Task Respond(Guid notificationId, bool isOkay)
        {
            var notif = await _dbContext.Notifications.Include(n => n.Task).ThenInclude(t => t!.User).FirstOrDefaultAsync(n => n.Id == notificationId) ?? throw new NotFoundException("Notification");
            var user = await _dbContext.Users.FindAsync(notif.ReceiverId) ?? throw new NotFoundException("User");


            var task = TaskMapper.ToModel(
                notif.Task!.GroupId,
                notif.Task.Name,
                notif.Task.DeadLine,
                notif.Task.PriorityCategory,
                notif.Task.StartTime,
                notif.Task.Repeat,
                notif.Task.Alarm,
                notif.Task.Description,
                notif.Task.SenderId);

            if (isOkay)
            {
                _dbContext.Tasks.Add(task);
                user.Tasks.Add(task);
                var messageForReciever = $"این تسک برای شما تنظیم شد";
                var notifForReceiver = new Notification(task, messageForReciever, NotificationType.System, notif.SenderId, notif.ReceiverId);
                user.Notifications!.Add(notifForReceiver);
                _dbContext.Notifications.Add(notifForReceiver);
            }
            _dbContext.Notifications.Remove(notif);

            string messageForSender = isOkay ? "درخواست شما پذیرفته شد" : "درخواست شما رد شد";
            var notifForSender = new Notification(task, messageForSender, NotificationType.Response, notif.ReceiverId, (Guid)notif.SenderId!);
            _dbContext.Notifications.Add(notifForSender);

            await _UnitOfWork.SaveChangesAsync();
        }
        public async Task SetSnooze(Guid notificationId, int minute)
        {
            var notif = await _dbContext.Notifications.FindAsync(notificationId) ?? throw new NotFoundException("Notification");
            notif.SetSnooze(minute);
        }
        public async Task DeleteNotification(Guid notificationid)
        {
            var notif = await _dbContext.Notifications.FindAsync(notificationid) ?? throw new NotFoundException("Notification");
            _dbContext.Notifications.Remove(notif);
            await _UnitOfWork.SaveChangesAsync();
        }
    }
}