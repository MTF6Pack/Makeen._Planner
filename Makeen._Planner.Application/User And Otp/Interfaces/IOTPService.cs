using Domain;
using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

namespace Application.UserAndOtp.Services
{
    public interface IOTPService
    {
        void SendOTP(string email);
        bool CheckOTP(string email, string userinput);
        Task<string> GenerateResetPasswordToken(User user);
        Task<IdentityResult> ResetPassword(User user, string token, string newpassword);
    }
}