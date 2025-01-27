using System.ComponentModel.DataAnnotations;

namespace Application.DataSeeder.OTP
{
    public interface IBaseEmailOTP
    {
        void Send(string email);
        bool CheckInput([EmailAddress] string email, string userInput);
    }
}