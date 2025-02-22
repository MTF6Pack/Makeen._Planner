using System.ComponentModel.DataAnnotations;

namespace Application.DataSeeder.OTP
{
    public interface IBaseEmailOTP
    {
        void SendAsync(string email);
        bool CheckInput([EmailAddress] string email, string userInput);
    }
}