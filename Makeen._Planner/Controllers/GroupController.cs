using Application.Group_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/groups")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;

        [HttpGet]
        [EndpointSummary("Fetches all groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            return Ok(await _groupService.GetAllAsync());
        }
        [HttpGet("{id}")]
        [EndpointSummary("Fetches a group by the groupid")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            return Ok(await _groupService.GetByIdAsync(id));
        }
        [HttpDelete("{id}")]
        [EndpointSummary("Deletes a group by the groupid")]
        public async Task<IActionResult> DeleteGroups(Guid id)
        {
            await _groupService.Delete(id);
            return Ok();
        }
        [HttpPost("{groupid}/users/{id}")]
        [EndpointSummary("Adds a user by userid, to a group by the groupid")]
        public async Task<IActionResult> AddUser(Guid groupid, Guid id)
        {
            await _groupService.AddMember(groupid, id);
            return Ok();
        }
        [Authorize]
        [HttpPost]
        [EndpointSummary("Creates a group")]
        public async Task<IActionResult> AddGroup([FromBody] AddGroupCommand command, [FromHeader] string token)
        {
            await _groupService.AddGroup(command, token);
            return Ok();
        }
        [HttpPut("{id}")]
        [EndpointSummary("Edites a group by the groupid")]
        public async Task<IActionResult> UpdateGroups(Guid id, [FromBody] UpdateGroupCommand command)
        {
            await _groupService.Update(id, command);
            return Ok();
        }
    }
}
