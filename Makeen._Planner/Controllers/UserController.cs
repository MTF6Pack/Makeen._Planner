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
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _userService.DeleteUser(id));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }
        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail([EmailAddress] string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateUserCommand command)
        {
            await _userService.UpdateUser(id, command);
            return Ok();
        }


        [HttpPost("tasks")]
        public async Task<IActionResult> AddTask([FromBody] AddTaskCommand command)
        {
            await _taskService.AddTask(command);
            return Ok();
        }

        [HttpPut("tasks/{id}")]
        public void UpdateTask(Guid id, [FromForm] UpdateTaskCommand command)
        {
            _taskService.UpdateTask(id, command);
        }
        [HttpGet("tasks/name")]
        public async Task<IActionResult> GetTaskByName(string name)
        {
            return Ok(await _taskService.GetObjectByName(name));
        }
        [HttpGet("tasks/{id}")]
        public async Task<IActionResult> GetAllUserTasks(Guid id)
        {
            return Ok(await _taskService.GetAllUserTasks(id));
        }
        [HttpDelete("tasks/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _taskService.RemoveTask(id);
            return Ok();
        }
    }
}