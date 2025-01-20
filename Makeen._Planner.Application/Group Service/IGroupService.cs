using Domain.Task;

namespace Application.Group_Service
{
    public interface IGroupService
    {
        System.Threading.Tasks.Task AddGroup(AddGroupCommand command);
        void AddGroup(Guid groupid);
        System.Threading.Tasks.Task AddUser(Guid groupId, Guid userId);
        Task<List<Group>> GetAllAsync();
        Task<Group> GetByIdAsync(Guid groupid);
        System.Threading.Tasks.Task Update(Guid id, UpdateGroupCommand command);
        System.Threading.Tasks.Task Delete(Guid id);
    }
}