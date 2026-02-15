using Microsoft.EntityFrameworkCore;
using childspace_backend.Models;
using childspace_backend.Models.Enums;

namespace childspace_backend.Data
{
    public class ChildSpaceDbContext : DbContext
    {
        public ChildSpaceDbContext(DbContextOptions<ChildSpaceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Center> Centers { get; set; }
        public DbSet<TrialRequest> TrialRequests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupChild> GroupChildren { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<UserChat> UserChats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasConversion<string>();

            modelBuilder.Entity<Center>()
                .Property(c => c.SubscriptionStatus).HasConversion<string>();

            modelBuilder.Entity<Attendance>()
                .Property(a => a.Status).HasConversion<string>();

            modelBuilder.Entity<Material>()
                .Property(m => m.Type).HasConversion<string>();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Center>()
                .HasMany(c => c.Users)
                .WithOne(u => u.Center)
                .HasForeignKey(u => u.CenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Center>()
                .HasMany(c => c.Children)
                .WithOne(ch => ch.Center)
                .HasForeignKey(ch => ch.CenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Center>()
                .HasMany(c => c.Groups)
                .WithOne(g => g.Center)
                .HasForeignKey(g => g.CenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Children)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Teacher)
                .WithMany(u => u.TeachingGroups)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Schedules)
                .WithOne(s => s.Group)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Materials)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupChild>()
                .HasOne(gc => gc.Group)
                .WithMany(g => g.GroupChildren)
                .HasForeignKey(gc => gc.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupChild>()
                .HasOne(gc => gc.Child)
                .WithMany(c => c.GroupChildren)
                .HasForeignKey(gc => gc.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Teacher)
                .WithMany() 
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Lesson)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Child)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Material>()
                .HasOne(m => m.Teacher)
                .WithMany()
                .HasForeignKey(m => m.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.UserChat)
                .WithMany(uc => uc.Messages)
                .HasForeignKey(m => m.UserChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}