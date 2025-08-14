using Application.Contracts.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using System.ComponentModel.DataAnnotations;
using static Persistence.Dapper;

namespace Makeen._Planner.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/notifications")]
    public class NotificationsController(DataBaseContext dbContext, INotificationService notificationService, NotificationQueryService dapper) : ControllerBase
    {
        private readonly DataBaseContext _dbContext = dbContext;
        private readonly INotificationService _notificationService = notificationService;
        private readonly NotificationQueryService _dapper = dapper;

        [HttpPost("{notificationid:guid}")]
        [EndpointSummary("Accept or reject the task request")]
        public async Task<IActionResult> Respond([FromRoute] Guid notificationid, bool isOkay)
        {
            await _notificationService.Respond(notificationid, isOkay);
            return Ok();
        }

        [HttpGet]
        [EndpointSummary("Fetches all notifs and marks them all as deliverd")]
        public async Task<IActionResult> GetAllNotifs([Required] bool beSorted)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await NotificationQueryService.GetUserNotificationsAsync(userid, false, beSorted));
        }

        [HttpGet("updates")]
        [EndpointSummary("fetches only undeliverd notifs and marks them as deliverd")]
        public async Task<IActionResult> GetUndeliverdNotifs()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await NotificationQueryService.GetUserNotificationsAsync(userid, true, false));
        }

        [HttpGet("{notificationId:guid}")]
        [EndpointSummary("fetches the detailed notification by Id")]
        public async Task<IActionResult> GetDueTask([FromRoute] Guid notificationId)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _notificationService.GetTheDueTask(userId, notificationId));
        }

        [HttpDelete("{notificationId:guid}")]
        [EndpointSummary("Deletes the notification by Id")]
        public async Task<IActionResult> DeleteNotif([FromRoute] Guid notificationId)
        {
            await _notificationService.DeleteNotification(notificationId);
            return Ok();
        }

        [HttpPatch("{notificationId:guid}")]
        [EndpointSummary("Sets Snooz for the notification")]
        public async Task<IActionResult> SetSnooz([FromRoute] Guid notificationId, [Required] int minute)
        {
            await _notificationService.SetSnooze(notificationId, minute);
            return Ok();
        }
    }
}