using Domains;
using Repository;
using Repository.Interface;

namespace Makeen._Planner.Task_Service
{
    public class TaskService(ITaskRepository repository, IUnitOfWork unitOfWork) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<Domains.Task>?> GetAllTasks()
        {
            return await _repository.GetAll();
        }

        public Domains.Task? GetObjectByName(string name)
        {
            Domains.Task? task = _repository.StraitAccess().FirstOrDefault(x => x.Name == name);
            return task;
        }

        public void RemoveTask(Guid id)
        {
            _repository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void UpdateTask(Guid id, UpdateTaskCommand command)
        {
            var thetask = _repository.GetObjectById(id);
            if (command != null)
            {
                thetask?.UpdateTask(command.Name, command.DeadLine, command.TaskCategory, command.PriorityCategory);
            }
        }
    }
}
