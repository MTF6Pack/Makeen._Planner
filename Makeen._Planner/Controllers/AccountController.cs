using Application.DataSeeder.OTP;
using Application.UserAndOtp.Services;
using Domain;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Application.DataSeeder;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Makeen._Planner.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController(IOTPService oTPService, IUserService userService, JwtToken jwt) : ControllerBase
    {
        private readonly IOTPService _oTPService = oTPService;
        private readonly IUserService _userService = userService;
        private readonly JwtToken _jwt = jwt;

        //[HttpGet("google-login")]
        //public IActionResult GoogleLogin(string returnUrl = "/")
        //{
        //    var redirectUrl = Url.Action("GoogleResponse", "https://192.168.1.156:6969/Account/google-response", null, Request.Scheme);
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse(User user)
        //{
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        //    //Console.WriteLine(JsonSerializer.Serialize(result.Principal.Claims.ToList()));
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(); // Handle failure
        //    }

        //    // Here you can create a JWT token and return it to the client
        //    var claims = result.Principal.Claims;
        //    // Generate JWT token logic here...
        //    await _userService.SigninByClaims(user, (System.Security.Claims.Claim)claims);

        //    return Ok(new { Token = claims });
        //}


        // هدایت به صفحه گوگل برای لاگین
        [HttpGet("google-login")]
        [EndpointSummary("Not working yet!")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // دریافت اطلاعات پس از ورود موفق
        [HttpGet("google-response")]
        [EndpointSummary("Not working yet!")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // اطلاعات کاربر
            var claims = result.Principal!.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // ذخیره اطلاعات کاربر در سیستم شما (در صورت نیاز)
            // و سپس هدایت به صفحه مورد نظر
            return LocalRedirect(returnUrl);
        }


        [HttpPost("SignUp")]
        [EndpointSummary("Registers a user")]
        public void SignUp([FromBody] AddUserCommand command)
        {
            _userService.SignUP(command);
        }
        [HttpPost("SigninByClaims/email")]
        [EndpointSummary("Login by email and password")]
        public void Signin([EmailAddress] string email, string password)
        {
            _userService.Signin(email, password);
        }
        [HttpPost("OTP")]
        [EndpointSummary("Sends an one-time-password to the email")]
        public void SendOTP(string email)
        {
            _oTPService.SendOTP(email);
        }
        [HttpGet("OTP-result")]
        [EndpointSummary("Verifies if the user input matches to the sent one-time-password")]
        public void CheckOTP(string email, string userinput)
        {
            _oTPService.CheckOTP(email, userinput);
        }
        [HttpPost("Token")]
        [EndpointSummary("Generates the requaierd token of Reset-Password api for the otp-verified user")]
        public async Task<IActionResult> GenerateResetPasswordToken(User user)
        { return Ok(await _oTPService.GenerateResetPasswordToken(user)); }
        [HttpPost("Reset-Password")]
        [EndpointSummary("Resets password for the otp-verified user (Token requaierd)")]
        public void ResetPassword(User user, string token, string newpassword)
        {
            _oTPService.ResetPassword(user, token, newpassword);
        }
    }
}
