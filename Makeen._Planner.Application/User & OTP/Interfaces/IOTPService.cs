using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

namespace Application.User.Services
{
    public interface IOTPService
    {
        void SendOTP(string email);
        bool CheckOTP(string email, string userinput);
        Task<Domain.User> FindUser(string email, bool isverified);
        Task<string> GenerateToken(Domain.User user);
        Task<IdentityResult> ResetPassword(Domain.User user, string token, string newpassword);
    }
}