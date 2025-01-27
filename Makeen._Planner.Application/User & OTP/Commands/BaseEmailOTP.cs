using MySqlX.XDevAPI;
using User = Domain.User;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Google.Protobuf.WellKnownTypes;

namespace Application.DataSeeder.OTP
{

    public class BaseEmailOTP : IBaseEmailOTP
    {
        private static readonly List<EmailOTPHelper> Valut = [];
        public void Send(string email)
        {

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

            string otp = GenerateOTP();
            EmailOTPHelper newOtp = new();
            Valut.Add(newOtp);

            message.Subject = "Reset Password";
            message.Body = $"Your Code is {otp}";

            client.Send(message);
        }
        private static string GenerateOTP()
        {
            Random random = new();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }
        public bool CheckInput([EmailAddress] string email, string userInput)
        {
            return Valut.FirstOrDefault(x => x.Email == email && userInput == x.Code) != null;
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



