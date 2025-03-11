using Domain;
using Infrustucture;
using Microsoft.AspNetCore.Http;
using Persistence.Repository.Interface;
using Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace Application.Group_Service
{
    public class GroupService(IGroupRepository repository, IUnitOfWork unitOfWork) : IGroupService
    {
        private readonly IGroupRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

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
                    AvatarUrl = g.AvatarUrl,
                    Color = g.Color,
                    Grouptype = g.Grouptype,
                    TotalTasks = g.Tasks != null ? g.Tasks.Count() : 0, // Null check to avoid potential null reference
                    DoneTasks = g.Tasks != null ? g.Tasks.Count(t => (int)t.Status == 1) : 0 // Null check to avoid potential null reference
                })
                .ToListAsync();

            return groupsWithTaskCounts;
        }

        public async Task<object> GetByIdAsync(Guid groupid)
        {
            var thegroup = await _repository.StraitAccess.Set<Group>().Where(g => g.Id == groupid).Select(g => new
            {
                g.Id,
                g.Title,
                g.Color,
                g.AvatarUrl,
                Members = g.GroupMemberships!.Select(m => new
                {
                    m.User.Fullname,
                    m.User.UserName,
                    m.User.AvatarUrl,
                    m.User.Email,
                    m.User.Id,
                    m.User.PhoneNumber,
                    m.IsAdmin
                }).ToList()
            }).FirstOrDefaultAsync();

            return thegroup ?? throw new NotFoundException(nameof(thegroup));
        }

        public async Task AddGroup(AddGroupCommand command, Guid ownerid)
        {
            Group newgroup = await command.ToModel(ownerid);

            newgroup.AddMember(ownerid, true); // Owner is admin by default

            await _repository.AddAsync(newgroup);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddMemberByEmail(AddUserByEmailDto request)
        {
            User? theuser = await _repository.StraitAccess.Set<User>().FirstOrDefaultAsync(u => u.Email == request.Email);
            Group? thegroup = await _repository.StraitAccess.Set<Group>().Include(x => x.GroupMemberships).FirstOrDefaultAsync(x => x.Id == request.Groupid);

            if (theuser != null && thegroup != null)
            {
                if (thegroup.GroupMemberships!.Any(m => m.UserId == theuser.Id))
                    throw new Exception("User is already a member of this group.");

                thegroup.GroupMemberships.Add(new GroupMembership(theuser.Id, thegroup.Id));
                await _unitOfWork.SaveChangesAsync();
            }
            else throw new NotFoundException("User or Group not found");
        }

        public async Task Update(Guid id, UpdateGroupCommand command)
        {
            string? newAvatarUrl = null;

            // Upload the avatar before fetching the group (prevents blocking DB access)
            if (command.AvatarUrl != null)
            {
                newAvatarUrl = await IFormFileToUrl.UploadFileAsync(command.AvatarUrl);
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
                    await IFormFileToUrl.DeleteFileAsync(oldAvatarUrl); // Ensure async execution
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleMemberToAdmin(Guid groupId, Guid userId)
        {
            var group = await _repository.GetByIdAsync(groupId) ?? throw new NotFoundException("Group not found.");
            var membership = await _repository.StraitAccess.Set<GroupMembership>().FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId) ?? throw new NotFoundException("Membership not found");
            if (group.OwnerId == userId) throw new InvalidOperationException("The group owner cannot be demoted.");
            membership.ToggleAdmin();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMember(Guid groupId, Guid userId)
        {
            var membership = await _repository.StraitAccess.Set<GroupMembership>().FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId) ?? throw new NotFoundException("Membership not found");
            _repository.StraitAccess.Set<GroupMembership>().Remove(membership);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
