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
    public class GroupService(IGroupRepository repository, IUnitOfWork unitOfWork, JwtTokenService jwt) : IGroupService
    {
        private readonly IGroupRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly JwtTokenService _jwt = jwt;

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
                g.AvatarUrl,
                Members = g.Members!.Select(m => m.UserName).ToList()
            })
    .FirstOrDefaultAsync();
            if (thegroup != null) return thegroup;
            else throw new NotFoundException(nameof(thegroup));
        }
        public async Task AddGroup(AddGroupCommand command, string token)
        {
            var claims = _jwt.ValidateToken(token) ?? throw new UnauthorizedException();
            var ownerid = JwtTokenService.GetUserIdFromPrincipal(claims);
            Group newgroup = command.ToModel(ownerid);
            await _repository.AddAsync(newgroup);
            await _unitOfWork.SaveChangesAsync();
            await AddMember(newgroup.Id, ownerid);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddMember(Guid groupId, Guid userId)
        {
            User? theuser = await _repository.StraitAccess.Set<User>().FindAsync(userId);
            Group? thegroup = await _repository.StraitAccess.Set<Group>().Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == groupId);

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
            thegroup.UpdateGroup(command.Title, command.AvatarUrl, command.Color);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
