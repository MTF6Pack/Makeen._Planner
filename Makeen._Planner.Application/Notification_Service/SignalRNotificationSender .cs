//using Domain.Services;
//using Infrastructure.SignalR;
//using Microsoft.AspNetCore.SignalR;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Application.Notification_Service
//{
//    public class SignalRNotificationSender(IHubContext<NotificationHub> hubContext) : INotificationSender
//    {
//        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

//        public async Task SendNotificationAsync(string userId, object payload, CancellationToken cancellationToken = default)
//        {
//            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", payload, cancellationToken);
//        }
//    }
//}