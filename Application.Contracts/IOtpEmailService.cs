using System.ComponentModel.DataAnnotations;

namespace Application.Contracts
{
    public interface IOtpEmailService
    {
        void SendOTPAsync(string email);
        bool CheckOTP([EmailAddress] string email, string userInput);
    }
}