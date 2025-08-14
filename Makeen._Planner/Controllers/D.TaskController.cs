using Application.Contracts.Tasks;
using Application.Contracts.Tasks.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                Message = hasConflict ? "Warning: There is a task conflict at this time! but the task has been added anyway" : "Task has been added successfully with no conflict",
                Conflict = hasConflict
            });
        }

        [HttpGet("calendar")]
        [EndpointSummary("Fetches tasks for the user or a group on a specific date")]
        public async Task<IActionResult> GetTheUserTasksByCalander([FromQuery] DateTime? date, [FromQuery] Guid? groupid, [FromQuery] bool isGrouptask)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetTheUserOrGroupTasksByCalander(date, userid, groupid, isGrouptask));
        }

        [HttpGet("admins/calendar")]
        [EndpointSummary("Fetches tasks for the user or a group on a specific date")]
        public async Task<IActionResult> GetAdminSentTasks([FromQuery] DateTime? date, [FromQuery] Guid groupid)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetAdminSentTasks(date, userid, groupid));
        }

        [HttpPatch("{taskid:guid}")]
        [EndpointSummary("Edits a task")]
        public async Task UpdateTask([FromRoute] Guid taskid, [FromBody] UpdateTaskCommand command)
        {
            await _taskService.UpdateTask(taskid, command);
        }

        [EndpointSummary("Marks the task as complete")]
        [HttpPatch("{taskid}/complete")]
        public async Task MarkTaskAsComplete([FromRoute] Guid taskid)
        {
            await _taskService.Done(taskid);
        }

        [EndpointSummary("Marks a list of tasks as complete")]
        [HttpPatch("complete")]
        public async Task MarkTaskAsComplete(List<Guid>? tasksid, DateTime? date)
        {
            await _taskService.Done(tasksid, date);
        }

        [HttpDelete("{taskid}")]
        [EndpointSummary("Deletes a task of the user by taskid")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid taskid)
        {
            await _taskService.RemoveTask(taskid);
            return Ok();
        }
    }
}
