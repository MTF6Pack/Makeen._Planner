using Makeen._Planner.Task_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Makeen._Planner.Controllers
{
    [Authorize]
    [Route("api/v1/tasks")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        private readonly ITaskService _taskService = taskService;

        [HttpPost]
        [EndpointSummary("Creates a task")]
        public async Task<IActionResult> AddTask([FromBody] AddTaskCommand command)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            bool hasConflict = await _taskService.AddTask(command, userId);

            return Ok(new
            {
                Message = hasConflict ? "Warning: You have a task conflict at this time! but the task added anyway" : "Task added successfully with no conflict",
                Conflict = hasConflict
            });
        }

        [HttpPatch("{taskid:guid}")]
        [EndpointSummary("Edits a task")]
        public async Task UpdateTask([FromRoute] Guid taskid, [FromBody] UpdateTaskCommand command)
        {
            await _taskService.UpdateTask(taskid, command);
        }

        //[HttpGet]
        //[EndpointSummary("Fetches all tasks of the user by token")]
        //public async Task<IActionResult> GetTheUserTasks()
        //{
        //    var userid = new Guid(User.FindFirst("id")!.Value);
        //    return Ok(await _taskService.GetAllUserTasks(userid));
        //}

        [HttpDelete("{taskid}")]
        [EndpointSummary("Deletes a task of the user by taskid")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid taskid)
        {
            await _taskService.RemoveTask(taskid);
            return Ok();
        }

        [EndpointSummary("Marks the task as complete")]
        [HttpPatch("{taskid}/complete")]
        public async Task MarkTaskAsComplete([FromRoute] Guid taskid)
        {
            await _taskService.Done(taskid);
        }

        [EndpointSummary("Marks a list of tasks as complete")]
        [HttpPatch("/complete")]
        public async Task MarkTaskAsComplete(List<Guid>? tasksid, DateOnly? date)
        {
            await _taskService.Done(tasksid, date);
        }

        //[HttpGet("{name}")]
        //[EndpointSummary("Fetches a task by the task name")]
        //public async Task<IActionResult> GetTaskByName([FromRoute] string name)
        //{
        //    return Ok(await _taskService.GetObjectByName(name));
        //}

        [HttpGet("calendar")]
        [EndpointSummary("Fetches tasks for the user or a group on a specific date")]
        public async Task<IActionResult> GetTheUserTasksByCalander([FromQuery] DateOnly? date, [FromQuery] Guid? groupid, [FromQuery] bool isGrouptask)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetTheUserOrGroupTasksByCalander(date, userid, groupid, isGrouptask));
        }

        //[HttpGet("All")]
        //[EndpointSummary("Fetches all tasks of all users")]
        //public async Task<IActionResult> GetAllTasks()
        //{
        //    return Ok(await _taskService.GetAllTasks());
        //}
    }
}
