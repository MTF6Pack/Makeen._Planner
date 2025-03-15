using Application.Group_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Makeen._Planner.Controllers
{
    [Authorize]
    [Route("api/v1/groups")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;

        [HttpGet]
        [EndpointSummary(" Fetches all groups for the authenticated user")]
        public async Task<IActionResult> GetAllGroupsOfUser()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _groupService.GetAllGroupsOfUser(userid));
        }

        [HttpGet("{groupId:guid}")]
        [EndpointSummary("Fetches a group by its ID")]
        public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
        {
            return Ok(await _groupService.GetByIdAsync(groupId));
        }

        [HttpDelete("{groupId:guid}")]
        [EndpointSummary("Deletes a group by its ID")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid groupId)
        {
            await _groupService.Delete(groupId);
            return Ok();
        }

        [HttpPost("{groupid:guid}/users/{email}")]
        [EndpointSummary("Adds a user to a group by email")]
        public async Task<IActionResult> AddUserByEmail([FromRoute] Guid groupid, [FromRoute, EmailAddress] string email)
        {
            await _groupService.AddMemberByEmail(groupid, email);
            return Ok();
        }

        [HttpPatch("{groupId:guid}/users/{userId:guid}/toggleadmin")]
        [EndpointSummary("Toggles a user's admin status in a group")]
        public async Task<IActionResult> ToggleAdmin([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            await _groupService.ToggleMemberToAdmin(groupId, userId);
            return Ok();
        }

        [HttpDelete("{groupId:guid}/users/{targetuserId:guid}")]
        [EndpointSummary("Removes a user from a group")]
        public async Task<IActionResult> RemoveMember([FromRoute] Guid groupId, [FromRoute] Guid targetuserId)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _groupService.RemoveMember(groupId, userid, targetuserId);
            return Ok();
        }

        [HttpPost]
        [EndpointSummary("Creates a group by token")]
        public async Task<IActionResult> AddGroup(AddGroupCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _groupService.AddGroup(command, userid);
            return Ok();
        }

        [HttpPatch("{groupId:guid}")]
        [EndpointSummary("Edits a group by the groupId")]
        public async Task<IActionResult> UpdateGroup([FromRoute] Guid groupId, UpdateGroupCommand command)
        {
            await _groupService.Update(groupId, command);
            return Ok();
        }
    }
}