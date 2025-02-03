using Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/charts")]
    [ApiController]
    public class ChartController(IChartService chartService) : ControllerBase
    {
        private readonly IChartService _chartService = chartService;
        [HttpGet]
        public Task<int> GetUserTasksCount(Guid userId)
        {
            return _chartService.GetUserStatus(userId);
        }
    }
}
