//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Threading.Tasks;

//namespace Infrastructure.SignalR
//{
//    public class NotificationHub : Hub
//    {
//        public override async Task OnConnectedAsync()
//        {
//            var userId = Context.User?.FindFirst("sub")?.Value;
//            if (!string.IsNullOrEmpty(userId))
//            {
//                UserConnectionManager.AddConnection(userId, Context.ConnectionId);
//                await base.OnConnectedAsync();
//            }
//        }

//        public override async Task OnDisconnectedAsync(Exception? exception)
//        {
//            UserConnectionManager.RemoveConnection(Context.ConnectionId);
//            await base.OnDisconnectedAsync(exception);
//        }
//    }
//}