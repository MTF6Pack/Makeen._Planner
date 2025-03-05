using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application
{
    public abstract class EmailServiceBase
    {
        protected static async Task SendEmailAsync(string email, string subject, string body)
        {
            using var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential("MTF.1380.2001@gmail.com", "njtx mzkk uepd mhts")
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress("MTF.1380.2001@gmail.com", "Makeen._Planner"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"{subject} sent to {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}