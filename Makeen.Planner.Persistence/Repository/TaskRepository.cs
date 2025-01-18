using Domain;
using Domain.Task;
using Persistence;
using Persistence.Repository.Base;
using Persistence.Repository.Interface;

namespace Persistence.Repository
{
    public class TaskRepository(DataBaseContext context) : Repository<Domain.Task.Task>(context), ITaskRepository
    {
    }
}
