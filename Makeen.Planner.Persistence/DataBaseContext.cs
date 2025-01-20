using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataBaseContext(DbContextOptions<DataBaseContext> options) : IdentityDbContext<User, UserRole, Guid>(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            //modelBuilder.UseEnumToStringConverter();
        }

        public static implicit operator DbSet<object>(DataBaseContext v)
        {
            throw new NotImplementedException();
        }
        //public required DbSet<User> Users { get; set; }
    }
}