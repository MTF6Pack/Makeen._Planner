using Makeen._Planner.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GateController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("SignUp")]
        public IActionResult AddUser([FromForm] AddUserCommand command)
        {
            return Ok(_userService.AddUser(command));
        }

        [HttpGet("Signin")]
        public async Task<IActionResult> Signin(/*[EmailAddress]*/ string username, string password)
        {
            return Ok(await _userService.GenerateToken(username, password));
        }

    }
}
