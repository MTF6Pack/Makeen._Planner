using Domains;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace Makeen._Planner.Service
{
    public interface IUserService
    {
        string AddUser(AddUserCommand command);
        Task<User?> GetUserById(Guid id);
        Task<string> GenerateToken(string username, string password);
        List<object>? GetAllUsers();
        void UpdateUser(Guid id, UpdateUserCommand command);
        Task<IdentityResult> DeleteUser(Guid id);
    }
}
