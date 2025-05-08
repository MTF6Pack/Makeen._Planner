using Application.Contracts;
using Application.Contracts.Groups;
using Domain;
using Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository;
using Persistence.Repository.Interface;
using Task = System.Threading.Tasks.Task;

namespace Application.Group_Service
{
    public class GroupService(IGroupRepository repository, IUnitOfWork unitOfWork, IFileStorageService avatar, GroupMapper groupMapper) : IGroupService
    {
        private readonly IGroupRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IFileStorageService _avatar = avatar;
        private readonly GroupMapper _groupMapper = groupMapper;

        public async Task Delete(Guid id)
        {
            var thegroup = await _repository.GetByIdAsync(id);
            if (thegroup == null) throw new NotFoundException(nameof(thegroup));
            _repository.Delete(thegroup);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Group>?> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<GroupWithTaskCountsDto>> GetAllGroupsOfUser(Guid userid)
        {
            var groupsWithTaskCounts = await _repository.StraitAccess.Set<Group>()
                .Where(g => g.GroupMemberships.Any(m => m.UserId == userid)) // Get groups where user is a member
                .Select(g => new GroupWithTaskCountsDto
                {
                    GroupId = g.Id,
                    GroupName = g.Title,
                    IsUserAdmin = g.GroupMemberships
                                   .Where(m => m.UserId == userid) // Filter memberships by userId
                                   .Select(m => m.IsAdmin) // Get the IsAdmin value from the targetMembership
                                   .FirstOrDefault(), // This should give the IsAdmin value for the user, or false if not found
                    AvatarUrl = g.AvatarUrl,
                    Color = g.Color,
                    Grouptype = g.Grouptype,
                    TotalTasks = g.Tasks != null ? g.Tasks.Count() : 0, // Null check to avoid potential null reference
                    DoneTasks = g.Tasks != null ? g.Tasks.Count(t => (int)t.Status == 1) : 0 // Null check to avoid potential null reference
                })
                .ToListAsync();

            return groupsWithTaskCounts;
        }

        public async Task<object> GetByIdAsync(Guid userid, Guid groupid)
        {
            var thegroup = await _repository.StraitAccess.Set<Group>().Where(g => g.Id == groupid).Select(g => new
            {
                g.Id,
                g.Title,
                g.Color,
                g.AvatarUrl,
                IsUserAdmin = g.GroupMemberships!.Any(gm => gm.IsAdmin && gm.UserId == userid),
                Members = g.GroupMemberships!.Select(m => new
                {
                    m.User.Fullname,
                    m.User.UserName,
                    m.User.AvatarUrl,
                    m.User.Email,
                    m.User.Id,
                    m.User.PhoneNumber,
                    m.IsAdmin
                }).ToList(),
            }).FirstOrDefaultAsync();

            return thegroup ?? throw new NotFoundException(nameof(thegroup));
        }

        public async Task AddGroup(AddGroupCommand command, Guid ownerid)
        {
            Group newgroup = await _groupMapper.ToModel(command, ownerid);

            newgroup.AddMember(ownerid, true); // Owner is admin by default

            await _repository.AddAsync(newgroup);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddMembers(Guid userid, Guid groupid, List<Guid> membersId)
        {
            var isAdmin = await _repository.StraitAccess.Set<GroupMembership>().AnyAsync(gm => gm.GroupId == groupid && gm.UserId == userid && gm.IsAdmin);
            if (!isAdmin) throw new UnauthorizedException("You are not admin");
            if (membersId == null || membersId.Count == 0) throw new BadRequestException("Member list cannot be null or empty.");

            var group = await _repository.GetByIdAsync(groupid) ?? throw new NotFoundException("Group");
            var existingMemberIds = await _repository.StraitAccess.Set<GroupMembership>()
                .Where(gm => gm.GroupId == groupid && membersId.Contains(gm.UserId))
                .Select(gm => gm.UserId)
                .ToListAsync();

            var newMembers = await _repository.StraitAccess.Set<User>()
                .Where(u => membersId.Contains(u.Id) && !existingMemberIds.Contains(u.Id))
                .ToListAsync();

            if (newMembers.Count == 0) throw new BadRequestException("All selected users are already members of this group.");
            var newMemberships = newMembers.Select(member => new GroupMembership(member.Id, groupid)).ToList();
            await _repository.StraitAccess.Set<GroupMembership>().AddRangeAsync(newMemberships);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                throw new BadRequestException("Some users are already members of this group.");
            }
        }

        public async Task Update(Guid id, UpdateGroupCommand command)
        {
            string? newAvatarUrl = null;

            // Upload the avatar before fetching the group (prevents blocking DB access)
            if (command.AvatarUrl != null)
            {
                newAvatarUrl = await _avatar.UploadFileAsync(command.AvatarUrl);
            }

            var thegroup = await _repository.GetByIdAsync(id);
            if (thegroup == null) throw new NotFoundException(nameof(thegroup));

            if (!string.IsNullOrWhiteSpace(command.Title))
                thegroup.UpdateTitle(command.Title);

            if (!string.IsNullOrWhiteSpace(command.Color))
                thegroup.UpdateColor(command.Color);

            if (newAvatarUrl != null)
            {
                string oldAvatarUrl = thegroup.AvatarUrl ?? string.Empty;
                thegroup.UpdateAvatar(newAvatarUrl);

                if (!string.IsNullOrEmpty(oldAvatarUrl))
                {
                    await _avatar.DeleteFileAsync(oldAvatarUrl); // Ensure async execution
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleMemberToAdmin(Guid groupId, Guid userId)
        {
            var group = await _repository.GetByIdAsync(groupId) ?? throw new NotFoundException("Group not found.");
            var membership = await _repository.StraitAccess.Set<GroupMembership>().FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId) ?? throw new NotFoundException("Membership not found");
            if (group.OwnerId == userId) throw new BadRequestException("The group owner cannot be demoted.");
            membership.ToggleAdmin();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMember(Guid groupId, Guid userid, Guid targetuserId)
        {
            var group = await _repository.GetByIdAsync(groupId) ?? throw new NotFoundException("Group");
            var targetMembership = await _repository.StraitAccess.Set<GroupMembership>()
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == targetuserId)
                ?? throw new NotFoundException("Membership not found");

            var callerMembership = await _repository.StraitAccess.Set<GroupMembership>()
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userid);
            if (callerMembership == null || !callerMembership.IsAdmin) throw new UnauthorizedException("You are not an admin");
            if (targetMembership.UserId == callerMembership.UserId) throw new BadRequestException("You cannot remove yourself");
            if (callerMembership.UserId != group.OwnerId && targetMembership.IsAdmin) throw new UnauthorizedException("Only group owner can remove an admins from group");
            _repository.StraitAccess.Set<GroupMembership>().Remove(targetMembership);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
