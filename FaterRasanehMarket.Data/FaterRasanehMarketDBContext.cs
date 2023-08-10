using FaterRasanehMarket.Data.Mapping;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.Entities.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FaterRasanehMarket.Data
{
    public class FaterRasanehMarketDBContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, RoleClaim, IdentityUserToken<int>>
    {
        public FaterRasanehMarketDBContext(DbContextOptions<FaterRasanehMarketDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.AddCustomIdentityMapping();
            builder.AddCustomMarketMappings();
            builder.Entity<User>().Ignore(c => c.AccessFailedCount)
                .Ignore(t => t.EmailConfirmed)
                .Ignore(c => c.LockoutEnabled)
                .Ignore(c => c.LockoutEnd)
                .Ignore(c => c.TwoFactorEnabled)
                .Property(c => c.RegisterDateTime).HasDefaultValueSql("CONVERT(datetime,GetDate())");
            builder.Entity<Role>().Ignore(c => c.ConcurrencyStamp);
            builder.Entity<Product>().Property(t=>t.AddDateTime).HasDefaultValueSql("CONVERT(datetime,GetDate())");
            builder.Entity<Visit>().Property(t => t.LastVisitDateTime).HasDefaultValueSql("CONVERT(datetime,GetDate())");
            builder.Entity<Order>().HasMany(b => b.OrderProducts).WithOne(p => p.Order).HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Address>().HasMany(b => b.Orders).WithOne(t=>t.Address).HasForeignKey(p => p.AddressId).OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<DayVisit> DayVisits { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
    }
}
