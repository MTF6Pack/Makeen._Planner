using Application.DataSeeder.OTP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Application.UserAndOtp
{
    public class OtpEmailService(IMemoryCache cache) : EmailServiceBase, IOtpEmailService
    {
        private readonly IMemoryCache _cache = cache;

        public async Task SendOTPAsync(string email)
        {
            _cache.Remove(email);
            string otp = GenerateOTP();
            _cache.Set(email, otp, TimeSpan.FromMinutes(2));
            await SendEmailAsync(email, "your OTP", otp);
        }
        public bool CheckOTP([EmailAddress] string email, string userInput)
        {
            if (_cache.TryGetValue(email, out string? otp) && otp == userInput)
            {
                _cache.Remove(email);
                return true;
            }
            return false;
        }
        private static string GenerateOTP()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString();
        }
    }
}