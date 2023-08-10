using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaterRasanehMarket.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Brand")]
        public int? BrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }
        public int CountLimit { get; set; }
        public int Price { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime AddDateTime { get; set; }
        public DateTime? LimitEnabledDateTime { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ICollection<OrderDetail> ProductOrders { get; set; }
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; }
    }
}
