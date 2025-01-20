using Makeen._Planner.Task_Service;
using Persistence.Repository.Interface;
using Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Task;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Group_Service
{
    public class GroupService(IGroupRepository repository, IUnitOfWork unitOfWork) : IGroupService
    {
        private readonly IGroupRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async System.Threading.Tasks.Task AddGroup(AddGroupCommand command)
        {
            Group newgroup = command.ToModel();
            await _repository.AddAsync(newgroup);
            await _unitOfWork.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task AddUser(Guid groupId, Guid userId)
        {
            User? founduser = await _repository.StraitAccess().Set<User>().FindAsync(userId);
            Group? thegroup = await _repository.StraitAccess().Set<Group>().Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == groupId);

            if (founduser != null && thegroup != null)
            {
                thegroup.Users!.Add(founduser);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task<Group> GetByIdAsync(Guid groupid)
        {
            var foundgroup = await _repository.GetByIdAsync(groupid);
            if (foundgroup != null) return foundgroup;
            else throw new Exception("Invalid Input");
        }

        public async Task<List<Group>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async System.Threading.Tasks.Task Update(Guid id, UpdateGroupCommand command)
        {
            var foundgroup = await GetByIdAsync(id);
            foundgroup.UpdateGroup(command.Title, command.AvatarUrl!, command.Color);
            await _unitOfWork.SaveChangesAsync();
        }

        public async void AddGroup(Guid groupid)
        {
            _repository.Delete(groupid);
            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task Delete(Guid id)
        {
            _repository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
