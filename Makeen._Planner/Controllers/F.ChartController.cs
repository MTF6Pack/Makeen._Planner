using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Makeen._Planner.Controllers
{
    [Authorize]
    [Route("api/v1/charts")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        [HttpGet]
        [EndpointSummary("Fetches recent week tasks charts of a user by token or a group by groupid")]
        public async Task<IActionResult> GetWeeklyReport([FromQuery] Guid? groupid = null)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            if (groupid == null) return Ok(await Persistence.Dapper.TasksReport(userid));
            return Ok(await Persistence.Dapper.TasksReport((Guid)groupid));
        }
    }
}