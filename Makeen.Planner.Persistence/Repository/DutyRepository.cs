using Makeen.Planner.Domain.Domains;
using Makeen.Planner.Persistence.Repository.Base;
using Makeen.Planner.Persistence.Repository.Interface;

namespace Makeen.Planner.Persistence.Repository
{
    public class DutyRepository(DataBaseContext context) : Repository<Duty>(context), IDutyRepository
    {
    }
}
