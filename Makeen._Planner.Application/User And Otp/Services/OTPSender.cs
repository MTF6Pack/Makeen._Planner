using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Application.DataSeeder.OTP;

namespace Application.UserAndOtp
{
    public class OTPSender(IMemoryCache cache) : IBaseEmailOTP
    {
        private readonly IMemoryCache _cache = cache;

        // Implementing the SendEmailAsync method from IEmailSender interface
        public async Task SendAsync(string email)
        {
            _cache.Remove(email);
            // Generate a unique confirmation token
            string otp = GenerateOTP();
            _cache.Set(email, otp, TimeSpan.FromMinutes(2)); // Cache token for 2 minutes

            // SMTP Client setup
            using var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential("MTF.1380.2001@gmail.com", "njtx mzkk uepd mhts") // Use a secure way to store credentials
            };

            // Creating the email message
            using var mailMessage = new MailMessage
            {
                From = new MailAddress("MTF.1380.2001@gmail.com", "Makeen._Planner"),
                Subject = "Here your OTP",
                Body = otp, // Using the messageBody parameter here
                IsBodyHtml = false // Set to true to send HTML content
            };

            mailMessage.To.Add(new MailAddress(email));

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"OTP sent to {email}");
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
            return random.Next(100000, 999999).ToString();
        }
    }
}