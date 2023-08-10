using FaterRasanehMarket.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaterRasanehMarket.Data.Mapping
{
   public class ProductDiscountMapping : IEntityTypeConfiguration<ProductDiscount>
    {
        public void Configure(EntityTypeBuilder<ProductDiscount> builder)
        {
            builder.HasKey(t => new { t.ProductId, t.DiscountId });
            builder
                .HasOne(p => p.Product)
                .WithMany(t => t.ProductDiscounts)
                .HasForeignKey(f => f.ProductId);
            builder
                .HasOne(p => p.Discount)
                .WithMany(t => t.ProductsDiscount)
                .HasForeignKey(f => f.DiscountId);
        }
    }
}
