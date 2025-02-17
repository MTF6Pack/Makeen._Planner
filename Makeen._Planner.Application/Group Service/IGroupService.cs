using Domain.Task;
using Task = System.Threading.Tasks.Task;

namespace Application.Group_Service
{
    public interface IGroupService
    {
        Task Delete(Guid id);
        Task<List<Group>?> GetAllAsync();
        Task<object> GetByIdAsync(Guid groupid);
        Task AddMember(Guid groupId, Guid userId);
        Task AddGroup(AddGroupCommand command);
        Task Update(Guid id, UpdateGroupCommand command);
    }
}