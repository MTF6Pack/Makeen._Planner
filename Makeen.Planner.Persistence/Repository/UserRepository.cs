using Domain;
using Domain.RepositoryInterfaces;
using Persistence.Repository.Base;

namespace Persistence.Repository
{
    public class UserRepository(DataBaseContext context) : Repository<User>(context), IUserRepository
    {
        public bool IsValidBase64(string base64String)
        {
            Span<byte> buffer = new(new byte[base64String.Length]);
            return Convert.TryFromBase64String(base64String, buffer, out _);
        }
    }
}