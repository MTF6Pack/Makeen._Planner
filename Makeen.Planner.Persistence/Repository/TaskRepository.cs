using Domains;
using Makeen.Planner.Persistence;
using Repository.Base;
using Repository.Interface;

namespace Repository
{
    public class TaskRepository(DataBaseContext context) : Repository<Domains.Task>(context), ITaskRepository
    {
    }
}
