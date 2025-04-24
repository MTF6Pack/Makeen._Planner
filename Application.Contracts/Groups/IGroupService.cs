using Domain;
using Task = System.Threading.Tasks.Task;

namespace Application.Contracts.Groups
{
    public interface IGroupService
    {
        Task Delete(Guid id);
        Task<List<Group>?> GetAllAsync();
        Task<object> GetByIdAsync(Guid userid, Guid groupid);
        Task AddGroup(AddGroupCommand command, Guid ownerid);
        Task Update(Guid id, UpdateGroupCommand command);
        Task RemoveMember(Guid groupId, Guid userid, Guid userId);
        Task ToggleMemberToAdmin(Guid groupId, Guid userId);
        Task<List<GroupWithTaskCountsDto>> GetAllGroupsOfUser(Guid userid);
        Task AddMembers(Guid userid, Guid groupid, List<Guid> membersId);
    }
}