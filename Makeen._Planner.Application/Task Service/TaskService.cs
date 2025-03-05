using Application.DataSeeder;
using Domain;
using Infrustucture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence;
using Persistence.Repository;
using Persistence.Repository.Interface;
using System;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace Makeen._Planner.Task_Service
{
    public class TaskService(ITaskRepository repository, IUnitOfWork unitOfWork) : ITaskService
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task AddTask(AddTaskCommand command, Guid userid)
        {
            if (command.DeadLine < DateTime.Now) throw new BadRequestException("Deadline cannot be in the past");
            User? theuser = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == userid);
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
            return await _repository.StraitAccess.Set<User>().Where(u => u.Id == userid).SelectMany(u => u.Tasks).ToListAsync();
        }
        public async Task<List<Domain.Task.Task>?> GetAllTasks()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<List<Domain.Task.Task>?> GetTheUserTasksByCalander(DateOnly date, Guid userid)
        {
            var theuserTasks = await _repository.StraitAccess.Set<User>().Where(u => u.Id == userid).Select(u => u.Tasks!.Where(t => (DateOnly.FromDateTime(t.DeadLine)) == date).ToList()).FirstOrDefaultAsync();
            return theuserTasks ?? throw new NotFoundException(nameof(theuserTasks));
        }

        public async Task<List<Domain.Task.Task>> GetObjectByName(string name)
        {
            List<Domain.Task.Task> tasks = await _repository.StraitAccess.Set<Domain.Task.Task>().Where(x => x.Name == name).ToListAsync();
            return tasks;
        }
        public async Task RemoveTask(Guid taskid)
        {
            Domain.Task.Task? thetask = await _repository.GetByIdAsync(taskid);
            if (thetask == null) throw new NotFoundException(nameof(thetask));
            _repository.Delete(thetask);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateTask(UpdateTaskCommand command, Guid userid)
        {
            Domain.Task.Task? thetask = await _repository.GetByIdAsync(userid);
            if (command != null) thetask?.UpdateTask(command.Name, command.DeadLine, command.PriorityCategory);
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
