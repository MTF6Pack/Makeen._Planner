using Application.DataSeeder.OTP;
using Domain;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Makeen._Planner.Service
{
    public interface IUserService
    {
        Task<string> SignUP(AddUserCommand command);
        Task<object?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
        Task<string> Signin(string username, string password);
        List<object>? GetAllUsers();
        Task UpdateUser(Guid id, UpdateUserCommand command);
        Task<IdentityResult> DeleteUser(Guid id);
        Task SigninByClaims(User user, Claim claims);
    }
}
