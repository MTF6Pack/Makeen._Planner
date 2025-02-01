using Application.DataSeeder.OTP;
using Application.UserAndOtp.Services;
using Domain;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController(IOTPService oTPService, IUserService userService) : ControllerBase
    {
        private readonly IOTPService _oTPService = oTPService;
        private readonly IUserService _userService = userService;

        [HttpPost("SignUp")]
        public IActionResult SignUp(AddUserCommand command)
        {
            _userService.SignUP(command);
            return Ok();
        }
        [HttpGet("Signin/email")]
        public async Task<IActionResult> Signin([EmailAddress] string email, string password)
        {
            return Ok(await _userService.Signin(email, password));
        }
        [HttpPost("OTP")]
        public void SendOTP(string email)
        { _oTPService.SendOTP(email); }
        [HttpGet("OTP-result")]
        public IActionResult CheckOTP(string email, string userinput)
        { return Ok(_oTPService.CheckOTP(email, userinput)); }
        [HttpPost("Token")]
        public async Task<string> GenerateResetPasswordToken(User user)
        { return await _oTPService.GenerateResetPasswordToken(user); }
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(User user, string token, string newpassword)
        { return Ok(await _oTPService.ResetPassword(user, token, newpassword)); }
    }
}
