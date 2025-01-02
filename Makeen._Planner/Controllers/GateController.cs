using Makeen._Planner.Service;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GateController(IUserService userService , IMediator mediator) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IMediator _mediator = mediator;

        [HttpPost("SignUp")]
        public IActionResult AddUser(AddUserCommand command)
        {
            return Ok(_mediator.Send(command));
        }

        [HttpGet("Signin")]
        public async Task<IActionResult> Signin(/*[EmailAddress]*/ string username, string password)
        {
            return Ok(await _userService.GenerateToken(username, password));
        }

    }
}
