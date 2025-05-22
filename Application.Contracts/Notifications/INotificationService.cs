using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Notifications
{
    public interface INotificationService
    {
        Task DeleteNotification(Guid notificationid);
        Task<object?> GetTheDueTask(Guid userId, Guid notificationid);
        Task Respond(Guid notificationId, bool isOkay);
        Task SetSnooze(Guid notificationId, int minute);
    }
}
