//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Makeen._Planner;
//using Persistence;
//using Infrastructure.SignalR; // or the appropriate namespace for NotificationHub

//namespace Makeen._Planner.Controllers
//{
//    [ApiController]
//    [Route("api/notifications")]

//    public class NotificationController(IHubContext<NotificationHub> hubContext) : ControllerBase
//    {
//        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

//        [HttpPost("sendtest")]
//        public async Task<IActionResult> SendTestNotification([FromQuery] string userId)
//        {
//            // Build your payload. For example, a dictionary of notifications:
//            var payload = new Dictionary<string, List<NotificationDto>>
//        {
//            {
//                "Reminder", new List<NotificationDto>
//                {
//                    new() {
//                        Id = Guid.NewGuid(),
//                        Message = "This is a test reminder notification",
//                        CreationTime = DateTime.Now,
//                        Result = "Pending",
//                        SenderInfo = new SenderInfo
//                        {
//                            SenderColor = "#FF5733",
//                            SenderName = "System",
//                            SenderUsername = "system_bot",
//                            SenderPhoto = "https://example.com/default-avatar.png"
//                        }
//                    }
//                }
//            }
//        };

//            // Use the global mapping to get the connection ID for the user
//            if (UserConnectionManager.TryGetConnection(userId, out var connectionId))
//            {
//                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", payload);
//                return Ok($"Notification sent to user {userId} (ConnectionId: {connectionId})");
//            }
//            else
//            {
//                return NotFound($"User {userId} is not connected.");
//            }
//        }
//    }
//}