using Makeen._Planner.Task_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }

        [HttpGet("MyProfile")]
        public IActionResult MyProfile(Guid Id)
        {
            return File(System.IO.File.ReadAllBytes(Id + ".png"), "image/png");
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateUserCommand command)
        {
            await _userService.UpdateUser(id, command);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        //[Authorize]
        [HttpGet("Get-All-Users")]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }
    }
}