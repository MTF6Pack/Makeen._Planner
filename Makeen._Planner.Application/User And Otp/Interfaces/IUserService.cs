using Application.DataSeeder.OTP;
using Domain;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Makeen._Planner.Service
{
    public interface IUserService
    {
        void SignUP(AddUserCommand command);
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
        Task<string> Signin(string username, string password);
        List<object>? GetAllUsers();
        Task UpdateUser(Guid id, UpdateUserCommand command);
        Task<IdentityResult> DeleteUser(Guid id);
    }
}
