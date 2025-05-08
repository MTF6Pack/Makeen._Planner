using Application.Contracts.Notifications;
using Application.Contracts.Tasks.Commands;
using Domain;
using Domain.Events;
using Infrastructure.Date_and_Time;
using Infrastructure.Exceptions;
using Infrastructure.SignalR;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Persistence.Repository;
using Task = System.Threading.Tasks.Task;

namespace Application.Notification_Service
{
    public class NotificationService(DataBaseContext dbContext, IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration, IUnitOfWork unitofwork, IMediator mediator, NotificationSenderHandler notificationSender) : INotificationService
    {
        private readonly DataBaseContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly string _defaultBaseUrl = configuration["BaseUrl"] ?? "https://your-default-domain.com";
        private readonly IUnitOfWork _UnitOfWork = unitofwork;
        private readonly IMediator _mediator = mediator;
        private readonly NotificationSenderHandler _notificationSender = notificationSender;

        private string GetBaseUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext != null
                ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}"
                : _defaultBaseUrl;
        }
        private object? GetSenderInfo(Guid? senderId, Guid? groupId, string defaultAvatarUrl)
        {
            var userInfo = senderId != null
                ? _dbContext.Users
                    .Where(user => user.Id == senderId)
                    .Select(user => new SenderInfoDto
                    {
                        SenderName = user.Fullname ?? "This field is null",
                        SenderUserName = user.UserName,
                        SenderAvatarUrl = user.AvatarUrl ?? defaultAvatarUrl
                    })
                    .FirstOrDefault()
                : null;

            var groupInfo = groupId != null
                ? _dbContext.Groups
                    .Where(group => group.Id == groupId)
                    .Select(group => new SenderInfoDto
                    {
                        SenderName = group.Title ?? "This field is null",
                        SenderUserName = null, // Groups don't have usernames
                        SenderAvatarUrl = group.AvatarUrl ?? defaultAvatarUrl,
                        SenderColor = group.Color
                    })
                    .FirstOrDefault()
                : null;

            if (userInfo != null && groupInfo != null)
            {
                return new
                {
                    User = userInfo,
                    Group = groupInfo
                };
            }
            return userInfo ?? groupInfo ?? null;
        }

        public async Task<object?> GetTheDueTask(Guid userId, Guid notificationId)
        {
            string baseUrl = GetBaseUrl();
            string defaultAvatarUrl = $"{baseUrl}/images/default-avatar.svg";

            // First, fetch the required fields from the database.
            var taskDueData = await _dbContext.Notifications
                .Include(n => n.Task)
                    .ThenInclude(t => t!.User)
                .Where(n => n.Id == notificationId)
                .OrderByDescending(n => n.Task!.CreationTime)
                .Select(n => new
                {
                    n.Id,
                    n.Type,
                    TaskId = n.Task!.Id,
                    CreationTime = DateHelper.ConvertGregorianToPersian(n.Task.CreationTime, true).persianDate,
                    DeadLine = DateHelper.ConvertGregorianToPersian(n.Task.DeadLine, true).persianDate,
                    n.Task.Name,
                    n.Task.Description,
                    n.Task.PriorityCategory,
                    n.Message,
                    n.Task.SenderId,
                    n.Task.GroupId
                })
                .FirstOrDefaultAsync();

            if (taskDueData == null)
                return null;

            // Now, compute SenderInfo on the client side.
            var senderInfo = GetSenderInfo(taskDueData.SenderId, taskDueData.GroupId, defaultAvatarUrl);

            // Return the complete object.
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
            }

            string message = isOkay ? "درخواست شما پذیرفته شد" : "درخواست شما رد شد";
            var notification = new Notification(task, message, NotificationType.Response, notif.Task.SenderId, notif.ReceiverId);
            user.Notifications!.Add(notification);
            _dbContext.Notifications.Add(notification);
            // Publish the TaskReminderEvent
            await _mediator.Publish(new TaskReminderEvent((Guid)notif.Task.SenderId!, notification));

            // Attempt to deliver the notification or queue it if the user is not connected
            await _notificationSender.HandleUndeliveredNotifications(CancellationToken.None);
            await _dbContext.SaveChangesAsync();
        }
    }
}