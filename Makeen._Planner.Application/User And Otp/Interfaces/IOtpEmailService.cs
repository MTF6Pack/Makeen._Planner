using Application.User_And_Otp.Commands;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Application.DataSeeder.OTP
{
    public interface IOtpEmailService
    {
        void SendOTPAsync(string email);
        bool CheckOTP([EmailAddress] string email, string userInput);
    }
}