using Infrustucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;


namespace Makeen._Planner.Controllers
{

    [Route("api/v1/charts")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [EndpointSummary("Fetches recent week tasks result of a user by token or a group by groupid ")]
        public async Task<IActionResult> GetWeeklyReport([FromQuery] Guid? groupid = null)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            if (groupid == null) return Ok(await Persistence.Dapper.TasksReport(userId));
            return Ok(await Persistence.Dapper.TasksReport((Guid)groupid));
        }

        [HttpGet("{groupid}")]
        [EndpointSummary("Fetches today group tasks by groupid")]
        public async Task<IActionResult> GetTodayGroupTasks(Guid groupid)
        {
            return Ok(await Persistence.Dapper.GroupTodayTasks(groupid));
        }
    }
}
