using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security.Claims;


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
//[HttpGet("donetasks")]
//[EndpointSummary("Fetches all donetasks of a user by token")]
//public async Task<IActionResult> GetUserDoneTasks()
//{
//    var userid = new Guid(User.FindFirst("id")!.Value);
//    return Ok(await Persistence.Dapper.UserDoneTasks(userid));
//}

//[HttpGet("futuretasks")]
//[EndpointSummary("Fetches all futuretasks of a user by token")]
//public async Task<IActionResult> GetUserFutureTasks()
//{
//    var userid = new Guid(User.FindFirst("id")!.Value);
//    return Ok(await Persistence.Dapper.UserFutureTasks(userid));
//}

//[HttpGet("failedtasks")]
//[EndpointSummary("Fetches all failedtasks of a user by token")]
//public async Task<IActionResult> GetUserFailedTasks()
//{
//    var userid = new Guid(User.FindFirst("id")!.Value);
//    return Ok(await Persistence.Dapper.UserFailedTasks(userid));
//}
