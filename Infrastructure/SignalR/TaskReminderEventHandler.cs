//using Domain.Events;
//using MediatR;
//using Microsoft.AspNetCore.SignalR;

//namespace Infrastructure.SignalR
//{
//    public class TaskReminderEventHandler(IHubContext<NotificationHub> hubContext) : INotificationHandler<TaskReminderEvent>
//    {
//        public async Task Handle(TaskReminderEvent notificationEvent, CancellationToken ct)
//        {
//            var userId = notificationEvent.UserId.ToString();

//            if (UserConnectionManager.TryGetConnections(userId, out var connections))
//            {
//                foreach (var connectionId in connections)
//                {
//                    await hubContext.Clients.Client(connectionId)
//                        .SendAsync("ReceiveNotification", notificationEvent.Notification, ct);
//                }
//            }
//            else
//            {
//                //    await notificationSender.QueueNotificationForLater(notificationEvent.Notification);
//                //}
//            }
//        }
//    }
//}