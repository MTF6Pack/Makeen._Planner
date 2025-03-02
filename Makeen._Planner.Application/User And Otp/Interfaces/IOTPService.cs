using Application.User_And_Otp.Commands;
using Domain;
using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

namespace Application.UserAndOtp.Services
{
    public interface IOTPService
    {
        Task SendOTP(string email);
        bool CheckOTP(string email, string userinput);
        Task<IdentityResult> ResetPassword(ForgetPasswordDto request);
    }
}