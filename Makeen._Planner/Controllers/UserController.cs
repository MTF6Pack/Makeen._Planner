using Makeen._Planner.Task_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Domain;

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
        [HttpGet("profile /{id}")]
        public IActionResult MyProfile(Guid id)
        {
            return File(System.IO.File.ReadAllBytes(id + ".png"), "image/png");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUser(id);
            return Ok();
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
        public async Task<IActionResult> AddTask([FromForm] AddTaskCommand command)
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
        public IActionResult GetTaskByName(string name)
        {
            return Ok(_taskService.GetObjectByName(name));
        }
        [HttpGet("tasks/{id}")]
        public IActionResult GetAllUserTasks(Guid id)
        {
            return Ok(_taskService.GetAllUserTasks(id));
        }
        [HttpDelete("tasks/{id}")]
        public void DeleteTask(Guid id)
        {
            _taskService.RemoveTask(id);
        }
    }
}