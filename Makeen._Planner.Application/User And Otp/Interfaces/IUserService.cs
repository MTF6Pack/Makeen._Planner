using Application.User_And_Otp.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Makeen._Planner.Service
{
    public interface IUserService
    {
        Task<string> SignUP(AddUserCommand command);
        Task<object?> GetUserById(Guid id);
        Task<object?> GetUserByEmail(string email);
        Task<string> Signin(SigninDto request);
        Task<List<object>> GetAllUsers();
        Task UpdateUser(UpdateUserCommand command, Guid userid);
        Task<IdentityResult> DeleteUser(Guid id);
        Task<IdentityResult> ResetPassword(ForgetPasswordDto request);
    }
}
