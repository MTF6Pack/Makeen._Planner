using Application.Contracts.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Authorize]
    [Route("api/v1/groups")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;

        [HttpPost("{groupid:guid}/users")]
        [EndpointSummary("Adds a list of users to a group by Id")]
        public async Task<IActionResult> AddMembers([FromRoute] Guid groupid, [FromBody] List<Guid> membersId)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _groupService.AddMembers(userid, groupid, membersId);
            return Ok(new { Message = "Members added successfully" });
        }

        [HttpPost]
        [EndpointSummary("Creates a group by token")]
        public async Task<IActionResult> AddGroup(AddGroupCommand command)
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            await _groupService.AddGroup(command, userid);
            return Ok();
        }

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
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _groupService.GetByIdAsync(userid, groupId));
        }

        [HttpPatch("{groupId:guid}/users/{userId:guid}/toggleadmin")]
        [EndpointSummary("Toggles a user's admin status in a group")]
        public async Task<IActionResult> ToggleAdmin([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            await _groupService.ToggleMemberToAdmin(groupId, userId);
            return Ok();
        }

        [HttpPatch("{groupId:guid}")]
        [EndpointSummary("Edits a group by the groupId")]
        public async Task<IActionResult> UpdateGroup([FromRoute] Guid groupId, UpdateGroupCommand command)
        {
            await _groupService.Update(groupId, command);
            return Ok();
        }

        [HttpDelete("{groupId:guid}")]
        [EndpointSummary("Deletes a group by its ID")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid groupId)
        {
            await _groupService.Delete(groupId);
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
    }
}