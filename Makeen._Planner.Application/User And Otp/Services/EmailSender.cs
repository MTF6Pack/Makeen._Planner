using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Application.EmailConfirmation
{
    public class EmailSender(IMemoryCache cache) : IEmailSender
    {
        private readonly IMemoryCache _cache = cache;

        // Implementing the SendEmailAsync method from IEmailSender interface
        public async Task SendEmailAsync(string email, string subject, string messageBody)
        {
            _cache.Remove(email);
            string confirmationToken = GenerateConfirmationToken();
            _cache.Set(email, confirmationToken, TimeSpan.FromMinutes(5)); // Cache token for 5 minutes

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
                Subject = subject,
                Body = messageBody, // Using the messageBody parameter here
                IsBodyHtml = true // Set to true to send HTML content
            };

            mailMessage.To.Add(new MailAddress(email));

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"Confirmation email sent to {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
            }
        }

        private static string GenerateConfirmationToken()
        {
            return Guid.NewGuid().ToString(); // Simple GUID as a token
        }

        // Optional method for validating the token
        public string ValidateConfirmationToken(string email, string userToken)
        {
            if (_cache.TryGetValue(email, out string? cachedToken))
            {
                if (cachedToken == userToken)
                {
                    _cache.Remove(email); // Token is valid, remove it
                    return "Email confirmed successfully! 🎉";
                }
                return "Invalid confirmation token! 🚫";
            }
            return "The confirmation email has expired. Please request a new one. ⏳";
        }

    }
}