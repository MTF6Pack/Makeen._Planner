using Domains;
using Makeen.Planner.Persistence;
using Repository.Base;
using Repository.Interface;

namespace Repository
{
    public class UserRepository(DataBaseContext context) : Repository<User>(context), IUserRepository
    {
    }
}
