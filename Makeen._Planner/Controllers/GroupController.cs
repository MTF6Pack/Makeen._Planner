using Application.Group_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/groups")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        private readonly IGroupService _groupService = groupService;
        [HttpPost]
        public async Task<IActionResult> AddGroup(AddGroupCommand command)
        {
            await _groupService.AddGroup(command);
            return Ok();
        }

        [HttpPost("users/{id}")]
        public async Task<IActionResult> AddUser(Guid groupid, Guid id)
        {
            await _groupService.AddUser(groupid, id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            return Ok(await _groupService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            return Ok(await _groupService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroups(Guid id, UpdateGroupCommand command)
        {
            await _groupService.Update(id, command);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroups(Guid id)
        {
            await _groupService.Delete(id);
            return Ok();
        }
    }
}
