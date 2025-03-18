using Infrastructure;
using Makeen._Planner.Task_Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Persistence.Repository;

namespace Application.Notification_Service
{
    public class NotificationService(DataBaseContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUnitOfWork unitofwork) : INotificationService
    {
        private readonly DataBaseContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly string _defaultBaseUrl = configuration["BaseUrl"] ?? "https://your-default-domain.com";
        private readonly IUnitOfWork _UnitOfWork = unitofwork;

        private string GetBaseUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext != null
                ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}"
                : _defaultBaseUrl;
        }
        private object GetSenderInfo(Guid? senderId, Guid? groupId, string defaultAvatarUrl)
        {
            var userInfo = senderId != null
                ? _dbContext.Users
                    .Where(user => user.Id == senderId)
                    .Select(user => new SenderInfoDto
                    {
                        SenderName = user.Fullname ?? "This field is null",
                        SenderUserName = user.UserName, // Groups don't have usernames, so this can be null
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

            // ✅ If both exist, return both in an anonymous object
            if (userInfo != null && groupInfo != null)
            {
                return new
                {
                    User = userInfo,
                    Group = groupInfo
                };
            }

            // ✅ If only one exists, return it
            return userInfo ?? groupInfo ?? new SenderInfoDto
            {
                SenderName = "زمان یار من",
                SenderUserName = null,
                SenderAvatarUrl = defaultAvatarUrl
            };
        }

        public async Task<object> GetDueTasks(Guid userId)
        {
            string baseUrl = GetBaseUrl();
            string defaultAvatarUrl = $"{baseUrl}/images/default-avatar.svg";

            // First, fetch the required fields from the database without calling GetSenderInfo.
            var notifications = await _dbContext.Notifications
                .Include(n => n.Task)
                    .ThenInclude(t => t!.User)
                .Where(n => n.Userid == userId && n.IsActivated)
                .OrderByDescending(n => n.Task!.CreationTime)
                .Select(n => new
                {
                    n.Id,
                    TaskId = n.Task!.Id,
                    CreationTime = DateHelper.ConvertGregorianToPersian(n.Task.CreationTime, true),
                    n.Message,
                    SenderId = n.Task.SenderId,
                    GroupId = n.Task.GroupId
                })
                .ToListAsync();

            // Now, on the client side, compute SenderInfo.
            var tasksDue = notifications.Select(n => new
            {
                n.Id,
                n.TaskId,
                n.CreationTime,
                n.Message,
                SenderInfo = GetSenderInfo(n.SenderId, n.GroupId, defaultAvatarUrl)
            }).ToList();

            return tasksDue;
        }
        public async Task<object?> GetTheDueTask(Guid userId, Guid notificationId)
        {
            string baseUrl = GetBaseUrl();
            string defaultAvatarUrl = $"{baseUrl}/images/default-avatar.svg";

            // First, fetch the required fields from the database.
            var taskDueData = await _dbContext.Notifications
                .Include(n => n.Task)
                    .ThenInclude(t => t!.User)
                .Where(n => n.Userid == userId && n.IsActivated && n.Id == notificationId)
                .OrderByDescending(n => n.Task!.CreationTime)
                .Select(n => new
                {
                    n.Id,
                    TaskId = n.Task!.Id,
                    CreationTime = DateHelper.ConvertGregorianToPersian(n.Task.CreationTime, true),
                    DeadLine = DateHelper.ConvertGregorianToPersian(n.Task.DeadLine, true),
                    n.Task.Name,
                    n.Task.Description,
                    n.Task.PriorityCategory,
                    n.Message,
                    SenderId = n.Task.SenderId,
                    GroupId = n.Task.GroupId
                })
                .FirstOrDefaultAsync();

            if (taskDueData == null)
                return null;

            // Now, compute SenderInfo on the client side.
            var senderInfo = GetSenderInfo(taskDueData.SenderId, taskDueData.GroupId, defaultAvatarUrl);

            // Return the complete object.
            return new
            {
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
        //public async Task DeleteNotification(Guid notificationid)
        //{
        //    var notif = await _dbContext.Notifications.FindAsync(notificationid) ?? throw new NotFoundException("Notification");
        //    _dbContext.Notifications.Remove(notif);
        //    await _UnitOfWork.SaveChangesAsync();
        //}

        public async Task Respond(Guid notificationid, bool isOkay)
        {
            if (isOkay)
            {
                var t = await _dbContext.Tasks.Include(t => t.User).FirstOrDefaultAsync(n => n.Id == notificationid) ?? throw new NotFoundException("Notification");
                var user = await _dbContext.Users.FindAsync(t.User!.Id) ?? throw new NotFoundException("User");
                var task = TaskMapper.ToModel(
                     t.GroupId,
                     t.Name,
                     t.DeadLine,
                     t.PriorityCategory,
                     t.StartTime,
                     t.Repeat,
                     t.Alarm,
                     t.Description,
                     t.SenderId);

                _dbContext.Tasks.Add(task);
                user.Tasks.Add(task);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}