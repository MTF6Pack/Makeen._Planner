using Infrustucture;
using Microsoft.AspNetCore.Mvc;


namespace Makeen._Planner.Controllers
{
    [Route("api/v1/charts")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeeklyReport(Guid id)
        {
            return Ok(await Persistence.Dapper.TasksReport(id) ?? throw new NotFoundException(nameof(id)));
        }
    }
}
