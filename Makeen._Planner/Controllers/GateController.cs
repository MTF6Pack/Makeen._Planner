using Application;
using Makeen._Planner.Service;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GateController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("SignUp")]
        public IActionResult SignUp(AddUserCommand command)
        {
            _userService.SignUP(command);
            return Ok();
        }

        [HttpGet("Signin")]
        public async Task<IActionResult> Signin([EmailAddress] string email, string password)
        {
            return Ok(await _userService.GenerateToken(email, password));
        }

    }
}
