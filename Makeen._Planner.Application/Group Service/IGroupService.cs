using Domain;
using System.ComponentModel.DataAnnotations;
using Task = System.Threading.Tasks.Task;

namespace Application.Group_Service
{
    public interface IGroupService
    {
        Task Delete(Guid id);
        Task<List<Group>?> GetAllAsync();
        Task<object> GetByIdAsync(Guid groupid);
        //Task AddMemberByEmail(Guid groupid, string email);
        Task AddGroup(AddGroupCommand command, Guid ownerid);
        Task Update(Guid id, UpdateGroupCommand command);
        Task RemoveMember(Guid groupId, Guid userid, Guid userId);
        Task ToggleMemberToAdmin(Guid groupId, Guid userId);
        Task<List<GroupWithTaskCountsDto>> GetAllGroupsOfUser(Guid userid);
        Task AddMembers(Guid userid, Guid groupid, List<Guid> membersId);
    }
}