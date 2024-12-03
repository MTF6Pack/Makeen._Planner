using Makeen.Planner.Domain.Domains;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Runtime.CompilerServices;

namespace Makeen._Planner.Service
{
    public interface IUserService
    {
        string AddUser(AddUserCommand command);
        Task<User?> GetUserById(Guid id);
        string GenerateToken(string email, string password);
        Task<List<User>?> GetAllUsers();
        void UpdateUser(Guid id, UpdateUserCommand command);
        void DeleteUser(Guid id, string password);
    }
}
