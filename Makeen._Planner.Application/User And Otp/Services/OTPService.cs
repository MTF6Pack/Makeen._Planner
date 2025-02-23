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

        public void SendOTP(string email) => _emailOTPService.SendAsync(email);
        public bool CheckOTP(string email, string userinput)
        {
            return _emailOTPService.CheckInput(email, userinput);
        }

        public async Task<IdentityResult> ResetPassword(string email, string newpassword)
        {
            var theuser = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User");
            var token = await _userManager.GeneratePasswordResetTokenAsync(theuser);
            var result = await _userManager.ResetPasswordAsync(theuser, token, newpassword);
            await _userManager.UpdateAsync(theuser);
            if (!result.Succeeded) throw new BadRequestException("Error");
            return IdentityResult.Success;
        }
    }
}