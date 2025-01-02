using Makeen._Planner.Duty_Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyController(IDutyService dutyService , IMediator mediator) : ControllerBase
    {
        private readonly IDutyService _dutyservice = dutyService;
        private readonly IMediator _mediator = mediator;


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
        public void UpdateDuty([FromForm] UpdateDutyCommand command)
        {
            _mediator.Send(command);
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
