using Domain;
using Infrustucture;
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
            User? theuser = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (theuser != null && command != null)
            {
                var task = command.ToModel();
                theuser.Tasks!.Add(task);
                _repository.StraitAccess.Set<Domain.Task.Task>().Add(task);
                await _unitOfWork.SaveChangesAsync();
            }
            else throw new NotFoundException(nameof(theuser) + " or " + nameof(command));
        }
        public async Task<List<Domain.Task.Task>> GetAllUserTasks(Guid userid)
        {
            var theuser = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == userid);
            return [.. theuser!.Tasks];
        }
        public async Task<Domain.Task.Task?> GetObjectByName(string name)
        {
            Domain.Task.Task? task = await _repository.StraitAccess.Set<Domain.Task.Task>().FirstOrDefaultAsync(x => x.Name == name);
            return task ?? throw new NotFoundException(nameof(task));
        }
        public async Task RemoveTask(Guid id)
        {
            Domain.Task.Task? thetask = await _repository.GetByIdAsync(id);
            if (thetask == null) throw new NotFoundException(nameof(thetask));
            _repository.Delete(thetask);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateTask(Guid id, UpdateTaskCommand command)
        {
            Domain.Task.Task? thetask = await _repository.GetByIdAsync(id);
            if (command != null) thetask?.UpdateTask(command.Name, command.DeadLine, command.TaskCategory, command.PriorityCategory);
            else throw new NotFoundException(nameof(thetask));
        }
        public async Task UpdateTaskStatus(Guid id, Domain.Task.TaskStatus status)
        {
            Domain.Task.Task? thetask = _repository.GetByIdAsync(id).Result;
            if (thetask != null) throw new NotFoundException(nameof(thetask));
            thetask?.UpdateTaskStatus(status);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
