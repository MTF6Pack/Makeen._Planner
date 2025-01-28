using Application.DataSeeder.OTP;
using Application.UserAndOtp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController(IOTPService oTPService) : ControllerBase
    {
        private readonly IOTPService _oTPService = oTPService;

        [HttpPost("Send-OTP")]
        public void SendOTP(string email)
        { _oTPService.SendOTP(email); }
        [HttpPost("Check-OTP")]
        public IActionResult CheckOTP(string email, string userinput)
        { return Ok(_oTPService.CheckOTP(email, userinput)); }
        [HttpGet("Find-UserAndOtp")]
        public async Task<IActionResult> FindUser(string email, bool isverified)
        { return Ok(await _oTPService.FindUser(email, isverified)); }
        [HttpPost("Generate-Token")]
        public async Task<string> GenerateToken(Domain.User user)
        { return await _oTPService.GenerateToken(user); }
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(Domain.User user, string token, string newpassword)
        { return Ok(await _oTPService.ResetPassword(user, token, newpassword)); }
    }
}
