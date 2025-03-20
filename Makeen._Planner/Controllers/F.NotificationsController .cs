using Application.Notification_Service;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace Makeen._Planner.Controllers
{
    [ApiController]
    [Route("api/v1/due-tasks")]
    public class NotificationsController(DataBaseContext dbContext, INotificationService notificationService) : ControllerBase
    {
        private readonly DataBaseContext _dbContext = dbContext;
        private readonly INotificationService _notificationService = notificationService;
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDueTasks()
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _notificationService.GetDueTasks(userId));
        }
        [Authorize]
        [HttpGet("{notificationid:guid}")]
        public async Task<IActionResult> GetDueTasks([FromRoute] Guid notificationid)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _notificationService.GetTheDueTask(userId, notificationid));
        }

        [HttpPost("{notificationid:guid}")]
        [EndpointSummary("Accept or reject the task request")]
        public async Task<IActionResult> Respond([FromRoute] Guid notificationid, bool isOkay)
        {
            await _notificationService.Respond(notificationid, isOkay);
            return Ok();
        }
        //[HttpDelete("due-tasks/{notificationid:guid}")]
        //public async Task<IActionResult> DeleteNotification([FromRoute] Guid notificationid)
        //{
        //    return Ok(await _notificationService.DeleteNotification(notificationid));

        //}
    }
}