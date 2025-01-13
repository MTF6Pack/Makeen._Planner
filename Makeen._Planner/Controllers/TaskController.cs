using Makeen._Planner.Task_Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService taskService, IMediator mediator) : ControllerBase
    {
        private readonly ITaskService _taskservice = taskService;
        private readonly IMediator _mediator = mediator;

        [HttpPost("Add-Task")]
        public void AddTask([FromBody] AddTaskCommand command)
        {
            _mediator.Send(command);
        }

        [HttpDelete("Delete-Task")]
        public void DeleteTask(Guid id)
        {
            _taskservice.RemoveTask(id);
        }

        [HttpPut("UpdateTask")]
        public void UpdateTask([FromForm] UpdateTaskCommand command)
        {
            _mediator.Send(command);
        }

        [HttpGet("GetAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            return Ok(await _taskservice.GetAllTasks());
        }
        [HttpGet("GetTask")]
        public IActionResult GetTask(string name)
        {
            return Ok(_taskservice.GetObjectByName(name));
        }
    }
}
