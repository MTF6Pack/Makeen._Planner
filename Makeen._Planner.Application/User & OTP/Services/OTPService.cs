using Application.DataSeeder.OTP;
using Application.User.Services;
using Domain;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto;
using System.Diagnostics.Eventing.Reader;

namespace Application.User.Services
{
    public class OTPService(UserManager<Domain.User> userManager, IBaseEmailOTP emailOTPService) : IOTPService
    {
        private readonly UserManager<Domain.User> _userManager = userManager;
        private readonly IBaseEmailOTP _emailOTPService = emailOTPService;

        public void SendOTP(string email) => _emailOTPService.Send(email);
        public bool CheckOTP(string email, string userinput)
        {
            if (_emailOTPService.CheckInput(email, userinput)) { return true; }
            else throw new Exception("Invalid input");
        }
        public async Task<Domain.User> FindUser(string email, bool isverified)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (isverified && user != null) return user;
            else throw new Exception("invalid input");
        }
        public async Task<string> GenerateToken(Domain.User user)
        { return await _userManager.GeneratePasswordResetTokenAsync(user); }
        public async Task<IdentityResult> ResetPassword(Domain.User user, string token, string newpassword)
        {
            await _userManager.ResetPasswordAsync(user, token, newpassword);
            await _userManager.UpdateAsync(user);
            return IdentityResult.Success;
        }
    }
}
//public class OTPService(UserManager<Domain.User> userManager, IEmailOTPService emailOTPService) : IOTPService
//{
//    private readonly UserManager<Domain.User> _userManager = userManager;
//    private readonly IEmailOTPService _emailOTPService = emailOTPService;

//    private EmailOTPHelper otp = new();
//    public void SendOTP(string email)
//    {
//        otp.Email = email;
//        _emailOTPService.Send(email);
//    }
//    public async Task<bool> CheckOTP(string userinput)
//    {
//        var user = await _userManager.FindByEmailAsync(otp.Email);
//        if (_emailOTPService.CheckInput(otp.Email, userinput) && user != null)
//        {
//            otp.User = user;
//            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//            otp.Token = token;
//            return true;
//        }
//        else throw new Exception("Invalid input");
//    }
//    public async Task<IdentityResult> ResetPassword(string newpassword)
//    {
//        if (otp.User != null && otp.Token != string.Empty)
//        {
//            await _userManager.ResetPasswordAsync(otp.User, otp.Token, newpassword);
//            await _userManager.UpdateAsync(otp.User);
//            return IdentityResult.Success;
//        }
//        else throw new Exception("Invalid Input");
//    }
//}
