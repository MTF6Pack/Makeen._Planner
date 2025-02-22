using Application.DataSeeder.OTP;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace Application.DataSeeder.OTP
{

    public class BaseEmailOTP(IMemoryCache cache) : IBaseEmailOTP
    {
        private readonly IMemoryCache _cache = cache;

        public void SendAsync(string email)
        {
            Task.Run(async () => await SendOtpInternal(email));
        }

        private async Task SendOtpInternal(string email)
        {
            string otp = GenerateOTP();
            _cache.Set(email, otp, TimeSpan.FromMinutes(5));

            using var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential("MTF.1380.2001@gmail.com", "njtx mzkk uepd mhts")
            };

            using var message = new MailMessage
            {
                From = new MailAddress("MTF.1380.2001@gmail.com", "MTF"),
                Subject = "Your OTP Code",
                Body = $"Your OTP code is: {otp}",
                IsBodyHtml = false
            };

            message.To.Add(new MailAddress(email));

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send OTP: {ex.Message}");
            }
        }

        public bool CheckInput(string email, string userInput)
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
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }
    }
}



