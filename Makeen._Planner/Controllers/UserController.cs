using Makeen._Planner.Duty_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("SignUp")]
        public IActionResult AddUser([FromForm] AddUserCommand command)
        {
            return Ok(_userService.AddUser(command));
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id, string password)
        {
            _userService.DeleteUser(id, password);
        }

        [HttpPut("UpdateUser")]
        public void Update(Guid id, [FromForm] UpdateUserCommand command)
        {
            _userService.UpdateUser(id, command);
        }

        [HttpGet("Login")]
        public IActionResult Login([EmailAddress] string email, string password)
        {
            return Ok(_userService.GenerateToken(email, password));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        //[Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }
    }
}
