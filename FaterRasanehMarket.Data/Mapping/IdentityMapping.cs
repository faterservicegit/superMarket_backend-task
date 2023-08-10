using FaterRasanehMarket.Entities.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FaterRasanehMarket.Data.Mapping
{
    public static class IdentityMapping
    {
        public static void AddCustomIdentityMapping(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("AppUsers");
            modelBuilder.Entity<Role>().ToTable("AppRoles");
            modelBuilder.Entity<UserRole>().ToTable("AppUserRole");
            modelBuilder.Entity<RoleClaim>().ToTable("AppRoleClaim");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("AppUserClaim");
            modelBuilder.Entity<UserRole>()
                .HasOne(u => u.User)
                .WithMany(ur => ur.Roles).HasForeignKey(t=>t.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.Role)
                .WithMany(u => u.Users).HasForeignKey(f => f.RoleId);
            modelBuilder.Entity<RoleClaim>()
                .HasOne(r => r.Role)
                .WithMany(c => c.Claims).HasForeignKey(f => f.RoleId);
        }
    }
}
