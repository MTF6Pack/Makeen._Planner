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

        public void Send(string email)
        {
            string otp = GenerateOTP();
            _cache.Set(email, otp, TimeSpan.FromMinutes(5));

            using var client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("MTF.1380.2001@gmail.com", "njtx mzkk uepd mhts");
            using var message = new MailMessage(
                from: new MailAddress("MTF.1380.2001@gmail.com", "MTF"),
                to: new MailAddress(email/*, username*/)
                );
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

    public class EmailOTPHelper
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public EmailOTPHelper()
        {

        }
    }
}



