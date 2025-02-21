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
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
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
        [HttpPut("{id}")]
        [EndpointSummary("Edits a user by userid")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            await _userService.UpdateUser(id, command);
            return Ok();
        }


        [HttpPost("tasks")]
        [EndpointSummary("Creates a task for a user by userid")]
        public async Task<IActionResult> AddTask([FromBody] AddTaskCommand command)
        {
            await _taskService.AddTask(command);
            return Ok();
        }
        [HttpPut("tasks/{id}")]
        [EndpointSummary("Edits a task for a user by userid")]
        public void UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
        {
            _taskService.UpdateTask(id, command);
        }
        [HttpGet("tasks/name")]
        [EndpointSummary("Fetches a task by the task name")]
        public async Task<IActionResult> GetTaskByName(string name)
        {
            return Ok(await _taskService.GetObjectByName(name));
        }
        [HttpGet("tasks/{id}")]
        [EndpointSummary("Fetches all tasks of a user by userid")]
        public async Task<IActionResult> GetTheUserTasks(Guid id)
        {
            return Ok(await _taskService.GetAllUserTasks(id));
        }
        [HttpGet("tasks/{id}/Calender")]
        [EndpointSummary("Fetches all tasks of a user in a specific date by user id and the date")]
        public async Task<IActionResult> GetTheUserTasksByCalander(Guid id, DateOnly date)
        {
            return Ok(await _taskService.GetTheUserTasksByCalander(id, date));
        }
        [HttpGet("tasks")]
        [EndpointSummary("Fetches all tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            return Ok(await _taskService.GetAllTasks());
        }
        [HttpDelete("tasks/{id}")]
        [EndpointSummary("Deletes a task of a user by userid")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _taskService.RemoveTask(id);
            return Ok();
        }
    }
}