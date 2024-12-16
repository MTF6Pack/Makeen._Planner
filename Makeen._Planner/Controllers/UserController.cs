using Makeen._Planner.Duty_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _userService.DeleteUser(id));
        }

        [HttpPut("UpdateUser")]
        public void Update(Guid id, [FromForm] UpdateUserCommand command)
        {
            _userService.UpdateUser(id, command);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        //[Authorize]
        [HttpGet()]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }
    }
}