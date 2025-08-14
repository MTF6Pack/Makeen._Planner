using Domain.RepositoryInterfaces;
using Persistence.Repository.Base;

namespace Persistence.Repository;
public class TaskRepository(DataBaseContext context) : Repository<Domain.Task>(context), ITaskRepository;
