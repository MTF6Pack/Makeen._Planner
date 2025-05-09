using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Group = Domain.Group;
using Task = Domain.Task;

namespace Persistence
{
    public class DataBaseContext : IdentityDbContext<User, UserRole, Guid>
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
            modelBuilder.Entity<GroupMembership>()
      .HasKey(gm => new { gm.UserId, gm.GroupId });

            modelBuilder.Entity<Task>()
                .HasMany(t => t.Instances)
                .WithOne()
                .HasForeignKey("TaskId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMembership>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMembership>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMemberships)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Task>()
      .Property(t => t.PriorityCategory)
      .HasColumnType("nvarchar(max)")
      .IsRequired(false); // Optional: make it nullable if needed

            modelBuilder.Entity<Task>()
        .Property(t => t.Repeat)
        .HasConversion<string>();

            modelBuilder.Entity<Task>()
       .Property(t => t.Alarm)
       .HasConversion<int>();
        }

        public required DbSet<Group> Groups { get; set; }
        public required DbSet<GroupMembership> GroupMemberships { get; set; }
        public required DbSet<Task> Tasks { get; set; }
        public required DbSet<Notification> Notifications { get; set; }
        public required DbSet<QueuedNotification> QueuedNotifications { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        public DataBaseContext()
        {

        }
    }
}