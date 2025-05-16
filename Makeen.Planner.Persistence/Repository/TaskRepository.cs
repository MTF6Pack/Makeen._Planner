using Persistence;
using Persistence.Repository.Base;
using Persistence.Repository.Interface;

namespace Persistence.Repository
{
    public class TaskRepository(DataBaseContext context) : Repository<Domain.Task>(context), ITaskRepository { }
}
