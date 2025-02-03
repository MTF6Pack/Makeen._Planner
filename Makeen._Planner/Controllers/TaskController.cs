using Makeen._Planner.Task_Service;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/tasks")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        private readonly ITaskService _taskservice = taskService;

        [HttpPost]
        public async Task<IActionResult> AddTask([FromForm] AddTaskCommand command)
        {
            await _taskservice.AddTask(command);
            return Ok();
        }

        [HttpDelete("{id}")]
        public void DeleteTask(Guid id)
        {
            _taskservice.RemoveTask(id);
        }

        [HttpPut("{id}")]
        public void UpdateTask(Guid id, [FromForm] UpdateTaskCommand command)
        {
            _taskservice.UpdateTask(id, command);
        }

        [HttpGet("{id}")]
        public IActionResult GetAllUserTasks(Guid id)
        {
            return Ok(_taskservice.GetAllUserTasks(id));
        }
        [HttpGet("name")]
        public IActionResult GetTaskByName(string name)
        {
            return Ok(_taskservice.GetObjectByName(name));
        }
    }
}
