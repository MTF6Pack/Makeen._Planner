using Makeen._Planner.Task_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/users")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateUserCommand command)
        {
            await _userService.UpdateUser(id, command);
            return Ok();
        }
        [HttpGet("profile /{id}")]
        public IActionResult MyProfile(Guid id)
        {
            return File(System.IO.File.ReadAllBytes(id + ".png"), "image/png");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail([EmailAddress] string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }
        //[Authorize]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }
    }
}