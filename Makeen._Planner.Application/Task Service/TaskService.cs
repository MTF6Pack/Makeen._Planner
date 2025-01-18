using Azure.Core;
using Domain.Task;
using Persistence.Repository;
using Persistence.Repository.Interface;

namespace Makeen._Planner.Task_Service
{
    public class TaskService(ITaskRepository repository, IUnitOfWork unitOfWork) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async System.Threading.Tasks.Task AddTask(AddTaskCommand command)
        {
            await _repository.AddAsync(command.ToModel());
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Domain.Task.Task>?> GetAllTasks()
        {
            return await _repository.GetAllAsync();
        }

        public Domain.Task.Task? GetObjectByName(string name)
        {
            Domain.Task.Task? task = _repository.StraitAccess().FirstOrDefault(x => x.Name == name);
            return task;
        }

        public async void RemoveTask(Guid id)
        {
            _repository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public void UpdateTask(Guid id, UpdateTaskCommand command)
        {
            Domain.Task.Task? thetask = _repository.GetByIdAsync(id).Result;
            if (command != null)
            {
                thetask?.UpdateTask(command.Name, command.DeadLine, command.TaskCategory, command.PriorityCategory);
            }
        }

        public void UpdateTaskStatus(Guid id, Domain.Task.TaskStatus status)
        {
            Domain.Task.Task? thetask = _repository.GetByIdAsync(id).Result;
            thetask?.UpdateTaskStatus(status);
        }
    }
}
