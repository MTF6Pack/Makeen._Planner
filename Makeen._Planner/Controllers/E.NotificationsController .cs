using Application.Contracts.Notifications;
using Application.Notification_Service;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using System.ComponentModel.DataAnnotations;
using static Persistence.Dapper;

namespace Makeen._Planner.Controllers
{
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

        [Authorize]
        [HttpGet]
        [EndpointSummary("Fetches all notifs and marks them all as deliverd")]
        public async Task<IActionResult> GetAllNotifs([Required] bool beSorted)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await NotificationQueryService.GetUserNotificationsAsync(userid, beSorted));
        }

        [Authorize]
        [HttpGet("updates")]
        [EndpointSummary("fetches only undeliverd notifs and marks them as deliverd")]
        public async Task<IActionResult> GetUndeliverdNotifs()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await NotificationQueryService.GetUserNotificationsAsync(userid, true, false));
        }

        [Authorize]
        [HttpGet("{notificationid:guid}")]
        public async Task<IActionResult> GetDueTask([FromRoute] Guid notificationid)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _notificationService.GetTheDueTask(userId, notificationid));
        }

        [Authorize]
        [HttpDelete("{notificationid:guid}")]
        public async Task<IActionResult> DeleteNotif([FromRoute] Guid notificationid)
        {
            await _notificationService.DeleteNotification(notificationid);
            return Ok();
        }
    }
}