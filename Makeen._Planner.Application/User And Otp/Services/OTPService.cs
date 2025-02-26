using Application.DataSeeder.OTP;
using Application.User_And_Otp.Commands;
using Domain;
using Infrustucture;
using Microsoft.AspNetCore.Identity;

namespace Application.UserAndOtp.Services
{
    public class OTPService(UserManager<User> userManager, IBaseEmailOTP emailOTPService) : IOTPService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IBaseEmailOTP _emailOTPService = emailOTPService;

        public async void SendOTP(string email)
        {
            var theuser = await _userManager.FindByEmailAsync(email);
            if (theuser == null) throw new NotFoundException(nameof(theuser));
            _emailOTPService.SendAsync(email);
        }
        public bool CheckOTP(string email, string userinput)
        {
            return _emailOTPService.CheckInput(email, userinput);
        }
        public async Task<IdentityResult> ResetPassword(ForgetPasswordDto request)
        {
            var theuser = await _userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException("User not found");
            var token = await _userManager.GeneratePasswordResetTokenAsync(theuser);
            var result = await _userManager.ResetPasswordAsync(theuser, token, request.Password);
            if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            return result;
        }
    }
}