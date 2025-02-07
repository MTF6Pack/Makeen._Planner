using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository;
using Persistence.Repository.Interface;
using Task = System.Threading.Tasks.Task;

namespace Makeen._Planner.Task_Service
{
    public class TaskService(ITaskRepository repository, IUnitOfWork unitOfWork) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task AddTask(AddTaskCommand command)
        {
            User? user = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (user != null && command != null)
            {
                var task = command.ToModel();
                user.Tasks!.Add(task);
                _repository.StraitAccess.Set<Domain.Task.Task>().Add(task);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public List<Domain.Task.Task> GetAllUserTasks(Guid userid)
        {
            var founduser = _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == userid).Result;
            return [.. founduser!.Tasks];
        }
        public Domain.Task.Task? GetObjectByName(string name)
        {
            Domain.Task.Task? task = _repository.StraitAccess.Set<Domain.Task.Task>().FirstOrDefault(x => x.Name == name);
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
            if (command != null) thetask?.UpdateTask(command.Name, command.DeadLine, command.TaskCategory, command.PriorityCategory);
        }
        public void UpdateTaskStatus(Guid id, Domain.Task.TaskStatus status)
        {
            Domain.Task.Task? thetask = _repository.GetByIdAsync(id).Result;
            thetask?.UpdateTaskStatus(status);
        }
    }
}
