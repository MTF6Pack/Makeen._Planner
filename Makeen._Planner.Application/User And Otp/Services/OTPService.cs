using Application.DataSeeder.OTP;
using Domain;
using Infrustucture;
using Microsoft.AspNetCore.Identity;

namespace Application.UserAndOtp.Services
{
    public class OTPService(UserManager<User> userManager, IBaseEmailOTP emailOTPService) : IOTPService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IBaseEmailOTP _emailOTPService = emailOTPService;

        public void SendOTP(string email) => _emailOTPService.Send(email);
        public bool CheckOTP(string email, string userinput)
        {
            if (_emailOTPService.CheckInput(email, userinput)) { return true; }
            else throw new BadRequestException("Invalid OTP provided.");
        }
        public async Task<string> GenerateResetPasswordToken(User user)
        { return await _userManager.GeneratePasswordResetTokenAsync(user); }
        public async Task<IdentityResult> ResetPassword(User user, string token, string newpassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newpassword);
            await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new BadRequestException("Error");
            return IdentityResult.Success;
        }
    }
}