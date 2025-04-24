using Application.Contracts;
using Application.Contracts.Users;
using Application.Contracts.Users.Commands;
using Domain;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.ComponentModel.DataAnnotations;
using Task = System.Threading.Tasks.Task;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController(IOtpEmailService otpEmailService, IUserService userService, UserManager<User> userManager, DataBaseContext context) : ControllerBase
    {
        private readonly IOtpEmailService _otpEmailService = otpEmailService;
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly DataBaseContext _Context = context;

        [HttpPost("SignUp")]
        [EndpointSummary("Registers a user and sends token")]
        public async Task<IActionResult> SignUp(AddUserCommand command)
        {
            return Ok(await _userService.SignUP(command));
        }

        [HttpPost("Signin/email")]
        [EndpointSummary("Login by email and password and sends token")]
        public async Task<IActionResult> Signin(SigninDto request)
        {
            return Ok(await _userService.Signin(request));
        }
        [HttpPost("OTP")]
        [EndpointSummary("Sends an one-time-password to the email")]
        public async Task<IActionResult> SendOTP([EmailAddress] string email)
        {
            var existinguser = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User");
            if (!existinguser.EmailConfirmed) throw new UnauthorizedException("Email is not confirmed");
            _otpEmailService.SendOTPAsync(email);
            return Ok(new { message = "OTP sent successfully" });
        }
        [HttpPost("OTP-result")]
        [EndpointSummary("Verifies if the user input matches to the sent one-time-password")]
        public IActionResult CheckOTP(CheckOtpDto dto)
        {
            return Ok(_otpEmailService.CheckOTP(dto.Email, dto.UserInput));
        }

        [HttpPost("Forget-OldPassword")]
        [EndpointSummary("Resets password; Use it only for the otp-verified user")]
        public async Task<IActionResult> ResetPassword(ForgetPasswordDto request)
        {
            return Ok(await _userService.ResetPassword(request));
        }

        [HttpGet("confirm-emails")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("User ID and token are required.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Invalid user ID.");


            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok("Email confirmed successfully!");

            return BadRequest("Email confirmation failed.");
        }
    }
}