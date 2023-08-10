using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FaterRasanehMarket.Entities
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }
        public int Percent { get; set; }
        public bool IsPublic { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ProductDiscount> ProductsDiscount { get; set; }
    }
}
