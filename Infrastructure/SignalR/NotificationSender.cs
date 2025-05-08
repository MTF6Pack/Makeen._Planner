using Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.SignalR
{
    public class NotificationSender(DataBaseContext dbContext)
    {
        public virtual async Task HandleUndeliveredNotifications(CancellationToken cancellationToken)
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
                SenderInfo? senderInfo = null;
                if (notification.Task!.GroupId.HasValue && notification.Task.SenderId is not null)
                {
                    var sender = await dbContext.Users
                        .Where(u => u.Id == notification.Task.SenderId)
                        .Select(u => new SenderInfo
                        {
                            SenderName = u.Fullname,
                            SenderUsername = u.UserName,
                            SenderPhoto = u.AvatarUrl,
                            /*  SenderColor = u.Memberships.*/ // Add this field to your User entity
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    senderInfo = sender;
                }

                var notificationDto = new NotificationDto
                {
                    Id = notification.Id,
                    Message = notification.Message!,
                    CreationTime = notification.CreationTime,
                    Result = notification.Task?.Result ?? "Pending",
                    SenderInfo = senderInfo
                };

                //notificationDtos.Add(notificationDto);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task QueueNotificationForLater(Notification notification)
        {
            var queuedNotification = new QueuedNotification
            {
                UserId = (Guid)notification.ReceiverId!,
                Id = notification.Id,
                RetryCount = 0,
                LastRetryTime = DateTime.UtcNow
            };

            await dbContext.QueuedNotifications.AddAsync(queuedNotification);
            await dbContext.SaveChangesAsync();
        }

        public async Task ResendQueuedNotification(QueuedNotification queued, CancellationToken ct)
        {
            var notification = await dbContext.Notifications
                .Include(n => n.Task)
                .FirstOrDefaultAsync(n => n.Id == queued.Id, ct);

            if (notification == null || notification.IsDelivered) return;

            if (UserConnectionManager.TryGetConnections(queued.UserId.ToString(), out var connections))
            {
                // Resend logic
                queued.RetryCount++;
                queued.LastRetryTime = DateTime.Now;
                await dbContext.SaveChangesAsync(ct);
            }
        }

        // Helper methods omitted for brevity
    }
}