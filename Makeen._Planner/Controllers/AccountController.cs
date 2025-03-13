using Application.DataSeeder.OTP;
using Application.User_And_Otp.Commands;
using Application.UserAndOtp;
using Application.UserAndOtp.Services;
using Azure.Core;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController(IOtpEmailService otpEmailService, IUserService userService, UserManager<User> userManager) : ControllerBase
    {
        private readonly IOtpEmailService _otpEmailService = otpEmailService;
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _userManager = userManager;

        // هدایت به صفحه گوگل برای لاگین
        //////////////////////[HttpGet("google-login")]
        //////////////////////[EndpointSummary("Not working yet!")]
        //////////////////////public IActionResult GoogleLogin(string returnUrl = "/")
        //////////////////////{
        //////////////////////    var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
        //////////////////////    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //////////////////////    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //////////////////////}

        //////////////////////// دریافت اطلاعات پس از ورود موفق
        //////////////////////[HttpGet("google-response")]
        //////////////////////[EndpointSummary("Not working yet!")]
        //////////////////////public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        //////////////////////{
        //////////////////////    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //////////////////////    // اطلاعات کاربر
        //////////////////////    var claims = result.Principal!.Identities.FirstOrDefault()?.Claims;
        //////////////////////    var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        //////////////////////    // ذخیره اطلاعات کاربر در سیستم شما (در صورت نیاز)
        //////////////////////    // و سپس هدایت به صفحه مورد نظر
        //////////////////////    return LocalRedirect(returnUrl);
        //////////////////////}

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
