using FaterRasanehMarket.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaterRasanehMarket.Data.Mapping
{
    public class LikeMapping : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(t => new { t.ProductId, t.UserId });
            builder
                .HasOne(p => p.Product)
                .WithMany(t => t.Likes)
                .HasForeignKey(f => f.ProductId);
            builder
                .HasOne(p => p.User)
                .WithMany(t => t.Likes)
                .HasForeignKey(f => f.UserId);
        }
    }
}
