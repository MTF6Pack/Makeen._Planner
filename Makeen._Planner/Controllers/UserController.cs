using Makeen._Planner.Task_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Domain;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserService userService, ITaskService taskService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ITaskService _taskService = taskService;

        [HttpGet]
        [EndpointSummary("Fetches all users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }
        [HttpDelete("{id}")]
        [EndpointSummary("Deletes a user by userid")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _userService.DeleteUser(id));
        }
        [HttpGet("{id}")]
        [EndpointSummary("Fetches a user by userid")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }
        [HttpGet("email")]
        [EndpointSummary("Fetches a user by email")]
        public async Task<IActionResult> GetByEmail([EmailAddress] string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }

        [Authorize]
        [HttpPut]
        [EndpointSummary("Edits the user")]
        public async Task<IActionResult> Update([FromBody] UpdateUserCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _userService.UpdateUser(command, userid);
            return Ok();
        }

        [Authorize]
        [HttpPost("tasks")]
        [EndpointSummary("Creates a task for the user")]
        public async Task<IActionResult> AddTask([FromBody] AddTaskCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _taskService.AddTask(command, userid);
            return Ok();
        }
        [Authorize]
        [HttpPut("tasks")]
        [EndpointSummary("Edits a task for the user")]
        public async Task UpdateTask([FromBody] UpdateTaskCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _taskService.UpdateTask(command, userid);
        }

        [HttpGet("tasks/name")]
        [EndpointSummary("Fetches a task by the task name")]
        public async Task<IActionResult> GetTaskByName(string name)
        {
            return Ok(await _taskService.GetObjectByName(name));
        }
        [Authorize]
        [HttpGet("tasks")]
        [EndpointSummary("Fetches all tasks of the user")]
        public async Task<IActionResult> GetTheUserTasks()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetAllUserTasks(userid));
        }
        [Authorize]
        [HttpGet("tasks/Calender")]
        [EndpointSummary("Fetches all tasks of the user in a specific date")]
        public async Task<IActionResult> GetTheUserTasksByCalander(DateOnly date)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetTheUserTasksByCalander(date, userid));
        }
        [HttpGet("tasks/All")]
        [EndpointSummary("Fetches all tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            return Ok(await _taskService.GetAllTasks());
        }
        [HttpDelete("tasks/{taskid}")]
        [EndpointSummary("Deletes a task of the user by taskid")]
        public async Task<IActionResult> DeleteTask(Guid taskid)
        {
            await _taskService.RemoveTask(taskid);
            return Ok();
        }
    }
}