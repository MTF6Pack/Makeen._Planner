//using Domain;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Task_Service
//{
//    public class AddNotificationCommand
//    {
//        public Domain.Task.Task? Task { get; set; }
//        public required string Message { get; set; }
//    }

//    public static class NotificationMapper
//    {
//        public static Notification ToMapper(this AddNotificationCommand command)
//        {
//            Notification newNotification = new(command.Task, command.Message);
//            return newNotification;
//        }
//    }
//}
