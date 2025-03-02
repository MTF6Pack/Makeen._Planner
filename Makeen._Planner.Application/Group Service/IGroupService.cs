using Domain;
using Task = System.Threading.Tasks.Task;

namespace Application.Group_Service
{
    public interface IGroupService
    {
        Task Delete(Guid id);
        Task<List<Group>?> GetAllAsync();
        Task<object> GetByIdAsync(Guid groupid);
        Task AddMember(Guid groupId, Guid userId);
        Task AddGroup(AddGroupCommand command, Guid ownerid);
        Task Update(Guid id, UpdateGroupCommand command);
    }
}