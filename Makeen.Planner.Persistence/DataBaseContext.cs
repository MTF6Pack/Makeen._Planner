using Makeen.Planner.Domain.Domains;
using Microsoft.EntityFrameworkCore;

namespace Makeen.Planner.Persistence
{
    public class DataBaseContext(DbContextOptions options) : DbContext(options)
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

        public DbSet<User>? Users { get; set; }
        public DbSet<Messege>? Messeges { get; set; }
    }
}