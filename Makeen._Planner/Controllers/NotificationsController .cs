using Application.Notification_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace Makeen._Planner.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController(DataBaseContext dbContext, INotificationService notificationService) : ControllerBase
    {
        private readonly DataBaseContext _dbContext = dbContext;
        private readonly INotificationService _notificationService = notificationService;
        [Authorize]
        [HttpGet("due-tasks")]
        public async Task<IActionResult> GetDueTasks()
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _notificationService.GetDueTasks(userId));
        }
        [Authorize]
        [HttpGet("due-tasks/{notificationid:guid}")]
        public async Task<IActionResult> GetDueTasks([FromRoute] Guid notificationid)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _notificationService.GetTheDueTask(userId, notificationid));
        }

        //[HttpDelete("due-tasks/{notificationid:guid}")]
        //public async Task<IActionResult> DeleteNotification([FromRoute] Guid notificationid)
        //{
        //    return Ok(await _notificationService.DeleteNotification(notificationid));

        //}
    }
}