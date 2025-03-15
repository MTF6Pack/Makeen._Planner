using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Makeen._Planner.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController(DataBaseContext dbContext) : ControllerBase
    {
        private readonly DataBaseContext _dbContext = dbContext;

        [HttpGet("due-tasks")]
        public async Task<IActionResult> GetDueTasks()
        {
            var userId = new Guid(User.FindFirst("id")!.Value);

            var tasksDue = await _dbContext.Notifications
                .Include(n => n.Task)
                    .ThenInclude(t => t!.User) // Include User
                .Where(n => n.Userid == userId && n.IsActivated == true)
                .OrderByDescending(n => n.Task!.CreationTime)
                .Select(n => new
                {
                    TaskId = n.Task!.Id,
                    TaskName = n.Task.Name,
                    TaskCreationTime = n.Task.CreationTime,
                    TaskDescription = n.Task.Description,
                    n.Message,

                    // Sender Details
                    Sender = n.Task.SenderId != null ? _dbContext.Users.Where(u => u.Id == n.Task.SenderId).Select(sender => new
                    {
                        FullName = sender.Fullname,
                        sender.AvatarUrl
                    }).FirstOrDefault() : null,
                    Group = n.Task.GroupId != null ? _dbContext.Groups.Where(g => g.Id == n.Task.GroupId).Select(g => new
                    {
                        GroupName = g.Title,
                        GroupAvatarUrl = g.AvatarUrl
                    }).FirstOrDefault() : null,

                    Default = (n.Task.SenderId == null && n.Task.GroupId == null)
                ? new
                {
                    DefaultName = "زمان یار من",
                    DefaultAvatarUrl = $"{Request.Scheme}://{Request.Host}/images/default-avatar.svg"
                }
                : null

                })
                .ToListAsync();

            return Ok(tasksDue);
        }
    }
}