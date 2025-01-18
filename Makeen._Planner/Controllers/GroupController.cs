using Application.Group_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;
        [HttpPost("Add-Groups")]
        public async Task<IActionResult> AddGroup(AddGroupCommand command)
        {
            await _groupService.AddGroup(command);
            return Ok();
        }

        [HttpGet("All-Groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            return Ok(await _groupService.GetAllAsync());
        }

        [HttpGet("Group-By-Id")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            return Ok(await _groupService.GetByIdAsync(id));
        }

        [HttpPut("Update-Groups")]
        public async Task<IActionResult> UpdateGroups(Guid id, UpdateGroupCommand command)
        {
            await _groupService.Update(id, command);
            return Ok();
        }

        [HttpDelete("Delete-Groups")]
        public async Task<IActionResult> DeleteGroups(Guid id)
        {
            await _groupService.Delete(id);
            return Ok();
        }
    }
}
