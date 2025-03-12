using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Makeen._Planner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController(DataBaseContext dbContext) : ControllerBase
    {
        private readonly DataBaseContext _dbContext = dbContext;

        [Authorize]
        [HttpGet("due-tasks")]
        public async Task<IActionResult> GetDueTasks()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            DateTime now = DateTime.Now;
            var tasksDue = await _dbContext.Notifications
                .Where(n => n.Userid == userid && n.IsActivated == true).OrderByDescending(n => n.Task!.CreationTime)
                .ToListAsync();

            return Ok(tasksDue);
        }
    }
}