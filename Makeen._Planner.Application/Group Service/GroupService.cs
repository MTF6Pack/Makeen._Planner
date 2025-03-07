using Application.DataSeeder;
using Domain;
using Infrustucture;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository;
using Persistence.Repository.Interface;
using Group = Domain.Group;
using Task = System.Threading.Tasks.Task;

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
        public async Task<object> GetByIdAsync(Guid groupid)
        {
            var thegroup = await _repository.StraitAccess.Set<Group>().Where(g => g.Id == groupid).Select(g => new
            {
                g.Id,
                g.Title,
                g.Color,
                g.AvatarUrl,
                Members = g.Members!.Select(m => new
                {
                    m.UserName,
                    m.AvatarUrl,
                    m.Email,
                    m.Id,
                    m.PhoneNumber
                }).ToList()
            })
    .FirstOrDefaultAsync();
            if (thegroup != null) return thegroup;
            else throw new NotFoundException(nameof(thegroup));
        }
        public async Task AddGroup(AddGroupCommand command, Guid ownerid)
        {
            Group newgroup = await command.ToModel(ownerid);
            await _repository.AddAsync(newgroup);
            await _unitOfWork.SaveChangesAsync();
            await AddMember(newgroup.Id, ownerid);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddMember(Guid groupid, Guid ownerid)
        {
            User? theuser = await _repository.StraitAccess.Set<User>().FindAsync(ownerid);
            Group? thegroup = await _repository.StraitAccess.Set<Group>().Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == groupid);

            if (theuser != null && thegroup != null)
            {
                thegroup.Members!.Add(theuser);
                await _unitOfWork.SaveChangesAsync();
            }

            else throw new NotFoundException(nameof(theuser) + " or/and " + nameof(thegroup));
        }

        public async Task AddMemberByEmail(AddUserByEmailDto request)
        {
            User? theuser = await _repository.StraitAccess.Set<User>().FirstOrDefaultAsync(u => u.Email == request.Email);
            Group? thegroup = await _repository.StraitAccess.Set<Group>().Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == request.Groupid);

            if (theuser != null && thegroup != null)
            {
                thegroup.Members!.Add(theuser);
                await _unitOfWork.SaveChangesAsync();
            }

            else throw new NotFoundException(nameof(theuser) + " or/and " + nameof(thegroup));
        }
        public async Task Update(Guid id, UpdateGroupCommand command)
        {
            var thegroup = await _repository.GetByIdAsync(id);
            if (thegroup == null) throw new NotFoundException(nameof(thegroup));

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(command.Title))
                thegroup.UpdateTitle(command.Title);

            if (!string.IsNullOrWhiteSpace(command.Color))
                thegroup.UpdateColor(command.Color);

            // Only update AvatarUrl if provided
            if (command.AvatarUrl != null)
            {
                string avatarUrl = await IformfileToUrl.UploadFile(command.AvatarUrl, id);
                thegroup.UpdateAvatar(avatarUrl);
            }

            // Commit changes using Unit of Work
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
