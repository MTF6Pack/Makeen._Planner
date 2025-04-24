using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Notifications
{
    public interface INotificationService
    {
        //Task<object?> DeleteNotification(Guid notificationid);
        //Task<object> GetDueTasks(Guid userid);
        Task<object?> GetTheDueTask(Guid userId, Guid notificationid);
        Task Respond(Guid notificationId, bool isOkay);
    }
}
