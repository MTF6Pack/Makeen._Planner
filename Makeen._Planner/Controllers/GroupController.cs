using Application.Group_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/groups")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;

        [Authorize]
        [HttpGet]
        [EndpointSummary("Fetches all groups of User")]
        public async Task<IActionResult> GetAllGroupsOfUser()
        {
            var userid = new Guid(User.FindFirst("id")!.Value);
            return Ok(await _groupService.GetAllGroupsOfUser(userid));
        }

        [HttpGet("{groupId}")]
        [EndpointSummary("Fetches a group by the groupId")]
        public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
        {
            return Ok(await _groupService.GetByIdAsync(groupId));
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Deletes a group by the groupId")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            await _groupService.Delete(id);
            return Ok();
        }

        [HttpPost("groupId/users")]
        [EndpointSummary("Adds a user to a group by email")]
        public async Task<IActionResult> AddUserByEmail([FromBody] AddUserByEmailDto request)
        {
            await _groupService.AddMemberByEmail(request);
            return Ok();
        }

        [HttpPatch("{groupId}/users/{userId}/toggleadmin")]
        [EndpointSummary("toggles a user to or from admin in a group")]
        public async Task<IActionResult> Togglradmin([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            await _groupService.ToggleMemberToAdmin(groupId, userId);
            return Ok();
        }

        [HttpDelete("{groupId}/users/{userId}")]
        [EndpointSummary("Removes a user from a group")]
        public async Task<IActionResult> RemoveMember([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            await _groupService.RemoveMember(groupId, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [EndpointSummary("Creates a group by token")]
        public async Task<IActionResult> AddGroup(AddGroupCommand command)
        {
            var userId = new Guid(User.FindFirst("id")!.Value);
            await _groupService.AddGroup(command, userId);
            return Ok();
        }

        [HttpPatch("{id}")]
        [EndpointSummary("Edits a group by the groupId")]
        public async Task<IActionResult> UpdateGroup(Guid id, UpdateGroupCommand command)
        {
            await _groupService.Update(id, command);
            return Ok();
        }
    }
}
