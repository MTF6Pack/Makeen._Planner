using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notification_Service
{
    public interface INotificationService
    {
        //Task<object?> DeleteNotification(Guid notificationid);
        Task<object> GetDueTasks(Guid userid);
        Task<object?> GetTheDueTask(Guid userId, Guid notificationid);
    }
}
