using FaterRasanehMarket.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaterRasanehMarket.Data.Mapping
{
    public class ProductCategoryMapping : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(t => new { t.ProductId, t.CategoryId });
            builder
                .HasOne(p => p.Product)
                .WithMany(t => t.ProductCategories)
                .HasForeignKey(f => f.ProductId);
            builder
                .HasOne(p => p.Category)
                .WithMany(t => t.ProductCategories)
                .HasForeignKey(f => f.CategoryId);
        }
    }
}
