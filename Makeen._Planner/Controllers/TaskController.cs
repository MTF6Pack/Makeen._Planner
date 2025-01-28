using Makeen._Planner.Task_Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        private readonly ITaskService _taskservice = taskService;

        [HttpPost("Add-Tasks")]
        public void AddTask([FromBody] AddTaskCommand command)
        {
            _taskservice.AddTask(command);
        }

        [HttpDelete("Delete-Tasks")]
        public void DeleteTask(Guid id)
        {
            _taskservice.RemoveTask(id);
        }

        [HttpPut("UpdateTask")]
        public void UpdateTask(Guid taskId, [FromForm] UpdateTaskCommand command)
        {
            _taskservice.UpdateTask(taskId, command);
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
