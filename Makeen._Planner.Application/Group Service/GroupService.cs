using Persistence.Repository.Interface;
using Persistence.Repository;
using Domain.Task;
using Domain;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Application.Group_Service
{
    public class GroupService(IGroupRepository repository, IUnitOfWork unitOfWork) : IGroupService
    {
        private readonly IGroupRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Delete(Guid id)
        {
            _repository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<Group>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Group> GetByIdAsync(Guid groupid)
        {
            var foundgroup = await _repository.StraitAccess.Set<Group>().AsNoTracking().Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == groupid);
            if (foundgroup != null) return foundgroup;
            else throw new Exception("Invalid Input");
        }
        public async Task AddGroup(AddGroupCommand command)
        {
            Group newgroup = command.ToModel();
            await _repository.AddAsync(newgroup);
            await AddMember(newgroup.Id, command.OwnerId);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddMember(Guid groupId, Guid userId)
        {
            User? founduser = await _repository.StraitAccess.Set<User>().FindAsync(userId);
            Group? thegroup = await _repository.StraitAccess.Set<Group>().Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == groupId);

            if (founduser != null && thegroup != null)
            {
                thegroup.Members!.Add(founduser);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task Update(Guid id, UpdateGroupCommand command)
        {
            var foundgroup = await GetByIdAsync(id);
            foundgroup.UpdateGroup(command.Title, (Guid)command.AvatarId!, command.Color);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
