using Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.SignalR
{
    public class NotificationSenderHandler(DataBaseContext dbContext, ILogger<NotificationSenderHandler> logger)
    {
        public async Task HandleUndeliveredNotifications(CancellationToken cancellationToken)
        {
            var undeliveredNotifications = await dbContext.Notifications
                .Include(n => n.Task)
                .Where(n => !n.IsDelivered)
                .ToListAsync(cancellationToken);

            // Batch load sender information
            var senderIds = undeliveredNotifications
                .Select(n => n.Task?.SenderId)
                .Where(id => id.HasValue)
                .Distinct()
                .ToList();

            var senders = await dbContext.Users
                .Where(u => senderIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            foreach (var notification in undeliveredNotifications)
            {
                try
                {
                    if (notification.Userid == null)
                    {
                        logger.LogWarning("Notification {NotificationId} has no user", notification.Id);
                        continue;
                    }

                    if (UserConnectionManager.TryGetConnections(notification.Userid.ToString()!, out var connectionIds))
                    {
                        var senderInfo = GetSenderInfo(notification, senders);
                        //var notificationDto = CreateNotificationDto(notification, senderInfo);

                        foreach (var connectionId in connectionIds)
                        {
                            //await hubContext.Clients.Client(connectionId)
                            //.SendAsync("ReceiveNotification", notificationDto, cancellationToken);
                        }

                        notification.Deliver();
                        logger.LogInformation("Delivered notification {NotificationId} to user {UserId}",
                            notification.Id, notification.Userid);
                    }
                    else
                    {
                        //await QueueNotificationForLater(notification);
                        logger.LogInformation("Queued notification {NotificationId} for offline user {UserId}",
                            notification.Id, notification.Userid);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process notification {NotificationId}", notification.Id);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static SenderInfo? GetSenderInfo(Notification notification, Dictionary<Guid, User> senders)
        {
            return notification.Task?.SenderId != null &&
                   senders.TryGetValue(notification.Task.SenderId.Value, out var sender)
                ? new SenderInfo
                {
                    SenderName = sender.Fullname,
                    SenderUsername = sender.UserName,
                    SenderPhoto = sender.AvatarUrl
                }
                : null;
        }
    }
}
