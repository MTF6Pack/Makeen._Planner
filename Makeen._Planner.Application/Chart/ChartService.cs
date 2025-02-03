using Domain;
using Domain.Report;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Migrations;
using Persistence.Repository;
using Persistence.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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