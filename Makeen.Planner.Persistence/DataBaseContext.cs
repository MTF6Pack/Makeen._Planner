using Domain;
using Domain.Task;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Group = Domain.Group;
using Task = Domain.Task.Task;

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
            modelBuilder.Entity<GroupMembership>()
      .HasKey(gm => new { gm.UserId, gm.GroupId });

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


            //modelBuilder.Entity<Task>()
            //.Property(e => e.CreationTime)
            //.HasDefaultValueSql("GETDATE()"); // Set default value to current date in SQL

            //modelBuilder.Entity<Task>()
            //    .Property(t => t.StartTime)
            //    .HasConversion(
            //        v => DateTime.SpecifyKind(v.AddHours(-3.5), DateTimeKind.Utc),  // ✅ Shift -3:30 before saving
            //        v => DateTime.SpecifyKind(v, DateTimeKind.Local)
            //    );

            //modelBuilder.Entity<Task>()
            //    .Property(t => t.DeadLine)
            //    .HasConversion(
            //        v => DateTime.SpecifyKind(v.AddHours(-3.5), DateTimeKind.Utc),  // ✅ Shift -3:30 before saving
            //        v => DateTime.SpecifyKind(v, DateTimeKind.Local)
            //    );
        }

        public required DbSet<Group> Groups { get; set; }
        public required DbSet<GroupMembership> GroupMemberships { get; set; }
        public required DbSet<Task> Tasks { get; set; }


    }
}