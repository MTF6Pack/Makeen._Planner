using Domains;
using Makeen.Planner.Persistence;
using Repository.Base;
using Repository.Interface;

namespace Repository
{
    public class DutyRepository(DataBaseContext context) : Repository<Duty>(context), IDutyRepository
    {
    }
}
