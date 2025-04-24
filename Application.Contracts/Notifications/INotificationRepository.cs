using Domain;
using Task = System.Threading.Tasks.Task;

namespace Application.Contracts.Notifications
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUndeliveredNotificationsAsync(CancellationToken cancellationToken);
        Task MarkNotificationAsDeliveredAsync(Guid notificationId, CancellationToken cancellationToken);
    }
}