using Domain;
using Persistence.Repository.Base;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Repository.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        bool IsValidBase64(string base64String);
    }
}
