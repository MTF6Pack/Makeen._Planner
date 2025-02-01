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
        public void AddTask([FromBody] AddTaskCommand command)
        {
            _taskservice.AddTask(command);
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

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            return Ok(await _taskservice.GetAllTasks());
        }
        [HttpGet("name")]
        public IActionResult GetTaskByName(string name)
        {
            return Ok(_taskservice.GetObjectByName(name));
        }
    }
}
