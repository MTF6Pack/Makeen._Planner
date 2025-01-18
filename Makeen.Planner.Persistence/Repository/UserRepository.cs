using Domain;
using Persistence;
using Persistence.Repository.Base;
using Persistence.Repository.Interface;

namespace Persistence.Repository
{
    public class UserRepository(DataBaseContext context) : Repository<User>(context), IUserRepository
    {
    }
}
