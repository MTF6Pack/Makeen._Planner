using Application.User_And_Otp.Commands;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Makeen._Planner.Task_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserService userService, ITaskService taskService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ITaskService _taskService = taskService;

        [HttpGet("All")]
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
        [Authorize]
        [HttpGet]
        [EndpointSummary("Fetches a user by token")]
        public async Task<IActionResult> GetById()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _userService.GetUserById(userid));
        }
        [HttpGet("email")]
        [EndpointSummary("Fetches a user by email")]
        public async Task<IActionResult> GetByEmail([EmailAddress] string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }

        [Authorize]
        [HttpPatch]
        [EndpointSummary("Edits the user by token")]
        public async Task<IActionResult> Update(UpdateUserCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _userService.UpdateUser(command, userid);
            return Ok();
        }
        [Authorize]
        [HttpPost("invite")]
        [EndpointSummary("invites a user to the app by email or phonenumber")]
        public async Task<IActionResult> Update(InviteUserDto request)
        {
            var userid = User.FindFirst("id")!.Value;
            await _userService.InviteFriend(request, userid);
            return Ok();
        }

        [Authorize]
        [HttpPost("contacts")]
        [EndpointSummary("Adds a user to user contacts by userid or email or username")]
        public async Task<IActionResult> AddContacts(AddContactDto request)
        {
            var theuserid = User.FindFirst("id")!.Value;
            await _userService.AddContact(theuserid, request);
            return Ok();
        }

        [Authorize]
        [HttpGet("contacts")]
        [EndpointSummary("get user contacts by token")]
        public async Task<IActionResult> GetContacts()
        {
            var theuserid = User.FindFirst("id")!.Value;
            return Ok(await _userService.GetContacts(theuserid));
        }

        [Authorize]
        [HttpDelete("contacts")]
        [EndpointSummary("Deletes a user from user contacts by target id")]
        public async Task<IActionResult> DeleteContacts(Guid targetid)
        {
            var theuserid = User.FindFirst("id")!.Value;
            await _userService.DeleteContact(theuserid, targetid);
            return Ok();
        }

        [Authorize]
        [HttpPost("tasks")]
        [EndpointSummary("Creates a task")]
        public async Task<IActionResult> AddTask([FromBody] AddTaskCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            //var useremail = User.FindFirst("email")!.Value;
            await _taskService.AddTask(command, userid);
            return Ok();
        }

        [Authorize]
        [HttpPatch("tasks")]
        [EndpointSummary("Edits a task for the user by token")]
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
        [EndpointSummary("Fetches all tasks of the user by token")]
        public async Task<IActionResult> GetTheUserTasks()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetAllUserTasks(userid));
        }
        [Authorize]
        [HttpGet("tasks/Calender")]
        [EndpointSummary("Fetches all tasks of the user in a specific date and token")]
        public async Task<IActionResult> GetTheUserTasksByCalander(DateOnly date)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _taskService.GetTheUserTasksByCalander(date, userid));
        }
        [HttpGet("tasks/All")]
        [EndpointSummary("Fetches all tasks of all users")]
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