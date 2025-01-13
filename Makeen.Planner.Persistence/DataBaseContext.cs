using Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Makeen.Planner.Persistence
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
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.UseEnumToStringConverter();
        }
        public DbSet<Domains.Task>? Tasks { get; set; }

        //public DbSet<Messege>? Messeges { get; set; }
    }
}