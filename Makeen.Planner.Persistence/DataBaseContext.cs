using Domain;
using Domain.Task;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Group = Domain.Task.Group;

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
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.UseEnumToStringConverter();
        }

        public required DbSet<Group> Groups { get; set; }
    }
}