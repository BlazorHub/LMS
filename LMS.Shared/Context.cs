using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Shared
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Course, Many to many
            modelBuilder.Entity<User>()
                .HasMany(p => p.Courses)
                .WithMany(p => p.Users)
                .UsingEntity<UserCourse>(
                    j => j
                        .HasOne(uc => uc.Course)
                        .WithMany(c => c.UserCourses)
                        .HasForeignKey(uc => uc.CourseId),
                    j => j
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.UserCourses)
                        .HasForeignKey(uc => uc.UserId),
                    j =>
                    {
                        j.HasKey(uc => new { uc.UserId, uc.CourseId });
                    });

            // For MYSQL UTF8MB4 Compatible, it was written around the time of ASP.NET Core 2.0, not sure if it is needed now, but it is there.
            //const int maxLength = 191;
            //modelBuilder
            //    .Entity<User>(e => e.Property(m => m.UserName).HasMaxLength(maxLength))
            //    .Entity<User>(e => e.Property(m => m.NormalizedUserName).HasMaxLength(maxLength))
            //    .Entity<User>(e => e.Property(m => m.Email).HasMaxLength(maxLength))
            //    .Entity<User>(e => e.Property(m => m.NormalizedEmail).HasMaxLength(maxLength))
            //    .Entity<IdentityRole<int>>(e => e.Property(m => m.Name).HasMaxLength(maxLength))
            //    .Entity<IdentityRole<int>>(e => e.Property(m => m.NormalizedName).HasMaxLength(maxLength))
            //    .Entity<IdentityUserLogin<int>>(e => e.Property(m => m.LoginProvider).HasMaxLength(maxLength))
            //    .Entity<IdentityUserLogin<int>>(e => e.Property(m => m.ProviderKey).HasMaxLength(maxLength))
            //    .Entity<IdentityUserToken<int>>(e => e.Property(m => m.LoginProvider).HasMaxLength(maxLength))
            //    .Entity<IdentityUserToken<int>>(e => e.Property(m => m.Name).HasMaxLength(maxLength));
        }
    }
}
