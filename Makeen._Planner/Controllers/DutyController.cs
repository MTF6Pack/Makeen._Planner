using Makeen._Planner.Duty_Service;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyController(IDutyService dutyService) : ControllerBase
    {
        private readonly IDutyService _dutyservice = dutyService;

        [HttpPost("AddDuty")]
        public void AddDuty([FromBody] AddDutyCommand command)
        {
            _dutyservice.AddDuty(command);
        }

        [HttpPost("DeleteDuty")]
        public void DeleteDuty(Guid id)
        {
            _dutyservice.RemoveDuty(id);
        }

        [HttpPut("UpdateDuty")]
        public void UpdateDuty(Guid id, [FromForm] UpdateDutyCommand command)
        {
            _dutyservice.UpdateDuty(id, command);
        }

        [HttpGet("GetAllDuties")]
        public async Task<IActionResult> GetAllDutes()
        {
            return Ok(await _dutyservice.GetAllDuties());
        }
        [HttpGet("GetDuty")]
        public IActionResult GetDuty(string name)
        {
            return Ok(_dutyservice.GetObjectByName(name));
        }
    }
}
