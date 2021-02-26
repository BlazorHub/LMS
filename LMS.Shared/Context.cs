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
            
            // For MYSQL UTF8MB4 Compatible, not sure if it is needed, but it is there.
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
