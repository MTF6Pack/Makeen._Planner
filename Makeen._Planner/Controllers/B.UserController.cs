using Application.Contracts.Users;
using Application.Contracts.Users.Commands;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Controllers
{
    [Authorize]
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("me")]
        [EndpointSummary("Fetches a user by token")]
        public async Task<IActionResult> GetById()
        {
            var token = User.FindFirst("id")!.Value ?? throw new UnauthorizedException();
            var userid = new Guid(token);
            return Ok(await _userService.GetUserById(userid));
        }

        [HttpPost("contacts")]
        [EndpointSummary("Adds a user to user contacts by userid or email or username")]
        public async Task<IActionResult> AddContacts(AddContactDto request)
        {
            var theuserid = User.FindFirst("id")!.Value;
            await _userService.AddContact(theuserid, request);
            return Ok();
        }

        [HttpGet("by-email")]
        [EndpointSummary("Fetches a user by email")]
        public async Task<IActionResult> GetByEmail([FromQuery, EmailAddress] string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }

        [HttpGet("contacts")]
        [EndpointSummary("get user contacts by token")]
        public async Task<IActionResult> GetContacts()
        {
            var theuserid = User.FindFirst("id")!.Value;
            return Ok(await _userService.GetContacts(theuserid));
        }

        [HttpPatch]
        [EndpointSummary("Edits the user by token")]
        public async Task<IActionResult> Update(UpdateUserCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _userService.UpdateUser(command, userid);
            return Ok();
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Deletes a user by userid")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _userService.DeleteUser(id));
        }

        [HttpDelete("contacts/{contactId}")]
        [EndpointSummary("Deletes a user from user contacts by contactid")]
        public async Task<IActionResult> DeleteContacts(Guid contactId)
        {
            var theuserid = User.FindFirst("id")!.Value;
            await _userService.DeleteContact(theuserid, contactId);
            return Ok();
        }
    }
}