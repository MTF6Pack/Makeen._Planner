using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository.Interface;

namespace Application.Chart
{
    public class ChartService(UserManager<User> userManager, IUserRepository userRepository) : IChartService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<int> GetUserStatus(Guid userid)
        {
            var founduser = await _userRepository.StraitAccess.Set<User>().Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == userid);
            if (founduser != null) return founduser.Tasks!.Count();
            throw new Exception("invalid info");
        }
    }
}