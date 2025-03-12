using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Task_Service
{
    public class AddNotificationCommand
    {
        public List<Domain.Task.Task>? Tasks { get; set; }
        public required string Message { get; set; }
    }

    public static class NotificationMapper
    {
        public static Notification ToMapper(this AddNotificationCommand command)
        {
            Notification newNotification = new(command.Tasks, command.Message);
            return newNotification;
        }
    }
}
