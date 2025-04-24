using Application.Contracts.Notifications;
using Domain;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Persistence.Repositories
{
    public class NotificationRepository(DataBaseContext dbContext) : INotificationRepository
    {
        private readonly DataBaseContext _dbContext = dbContext;

        public async Task<List<Notification>> GetUndeliveredNotificationsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Notifications
                .Where(n => !n.IsDelivered)
                .ToListAsync(cancellationToken);
        }

        public async Task MarkNotificationAsDeliveredAsync(Guid notificationId, CancellationToken cancellationToken)
        {
            var notification = await _dbContext.Notifications.FindAsync([notificationId], cancellationToken);
            if (notification != null)
            {
                notification.Deliver();
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}