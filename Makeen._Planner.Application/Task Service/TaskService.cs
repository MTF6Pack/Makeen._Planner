using Application.DataSeeder;
using Domain;
using Infrustucture;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            if (command.DeadLine < DateTime.Now || command.StartTime < DateTime.Now) throw new BadRequestException("Deadline nor Start time cannot be in the past");
            if (command.DeadLine < command.StartTime) throw new BadRequestException("Deadline cannot be before Starttime");
            if (command.ReceiveId == null && command.GroupId == null) throw new BadRequestException("Select At least a group or a user");

            bool? isREALLYInGroup = command.GroupId.HasValue ? await _repository.StraitAccess.Set<Group>().Where(g => g.Id == command.GroupId)
                 .AsNoTracking().Select(g => (bool?)g.Grouptype).FirstOrDefaultAsync() : null;

            bool isGroupExists = isREALLYInGroup.HasValue;
            bool isGroupPublic = isREALLYInGroup == true; // If the group exists, check its type.
            bool hasGroupId = command.GroupId.HasValue;
            bool hasReceiver = command.ReceiveId.HasValue;

            bool AdminIsSendingATask_ForAUserInsideAGroup = isGroupPublic && hasGroupId && hasReceiver;
            bool AdminIsSendingATask_ForALLUsersInsideAGroup = isGroupPublic && hasGroupId && !hasReceiver;
            bool UserIsSendingATask_ForAUserOutsideGroup = isGroupPublic && !hasGroupId && hasReceiver;
            bool UserIsSendingATask_ForHisList = !isGroupPublic && hasGroupId && !hasReceiver;
            bool UserIsCreatingATask_ForHimself = !isGroupPublic && !hasGroupId && !hasReceiver;
            Guid? senderid = !UserIsCreatingATask_ForHimself ? userid : null;
            User? targetuser = null;

            if (AdminIsSendingATask_ForAUserInsideAGroup)
            {
                bool isAdmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(g => g.GroupId == command.GroupId && g.UserId == userid && g.IsAdmin);
                if (!isAdmin) throw new UnauthorizedException("User is not an admin in this group");
                targetuser = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.UserName == command.ReceiverUserName) ?? throw new NotFoundException("User");
            }
            else if (AdminIsSendingATask_ForALLUsersInsideAGroup)
            {
                bool isAdmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(g => g.GroupId == command.GroupId && g.UserId == userid && g.IsAdmin);
                if (!isAdmin) throw new UnauthorizedException("User is not an admin in this group");
            }

            else if (UserIsSendingATask_ForAUserOutsideGroup)
            {
                targetuser = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.UserName == command.ReceiverUserName) ?? throw new NotFoundException("User");
                //Something must be done here
                return;
            }

            else if (UserIsCreatingATask_ForHimself) targetuser = await _repository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == userid);

            else throw new BadRequestException();
            var task = command.ToModel(senderid);
            if (targetuser != null) targetuser.Tasks!.Add(task);
            _repository.StraitAccess.Set<Domain.Task.Task>().Add(task);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<Domain.Task.Task>> GetAllUserTasks(Guid userid)
        {
            return await _repository.StraitAccess.Set<User>().Where(u => u.Id == userid).SelectMany(u => u.Tasks).OrderByDescending(t => t.CreationTime).ToListAsync();
        }
        public async Task<List<Domain.Task.Task>?> GetAllTasks()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<List<Domain.Task.Task>?> GetTheUserTasksByCalander(DateOnly date, Guid userid)
        {
            var theuserTasks = await _repository.StraitAccess.Set<User>().Where(u => u.Id == userid).Select(u => u.Tasks!.Where(t => (DateOnly.FromDateTime(t.StartTime)) == date).OrderByDescending(t => t.CreationTime).ToList()).FirstOrDefaultAsync();
            return theuserTasks ?? throw new NotFoundException(nameof(theuserTasks));
        }

        public async Task<List<Domain.Task.Task>> GetObjectByName(string name)
        {
            List<Domain.Task.Task> tasks = await _repository.StraitAccess.Set<Domain.Task.Task>().Where(x => x.Name == name).OrderByDescending(t => t.CreationTime).ToListAsync();
            return tasks;
        }
        public async Task RemoveTask(Guid taskid)
        {
            Domain.Task.Task? thetask = await _repository.GetByIdAsync(taskid);
            if (thetask == null) throw new NotFoundException(nameof(thetask));
            _repository.Delete(thetask);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateTask(UpdateTaskCommand command, Guid taskId)
        {
            // Fetch the task using taskId
            Domain.Task.Task? thetask = await _repository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task");

            // Directly use PriorityCategory if it's non-nullable
            thetask.UpdateTask(command.Name, command.DeadLine, command.PriorityCategory, command.StartTime, command.Repeat, command.Alarm, command.Description);

            // Commit changes using Unit of Work
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateTaskStatus(Guid id, Domain.Task.TaskStatus status)
        {
            Domain.Task.Task? thetask = _repository.GetByIdAsync(id).Result;
            if (thetask != null) throw new NotFoundException(nameof(thetask));
            thetask?.UpdateTaskStatus(status);
            await _unitOfWork.SaveChangesAsync();
        }

        //public async Task SendNotification(Guid userid,)
    }
}
