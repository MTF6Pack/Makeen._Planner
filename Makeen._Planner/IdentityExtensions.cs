using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Makeen._Planner
{
    public static class IdentityExtensions
    {
        public static void AddCustomIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, UserRole>().AddEntityFrameworkStores<DataBaseContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = "id";
                options.ClaimsIdentity.UserNameClaimType = "email";
            });
        }
    }
}