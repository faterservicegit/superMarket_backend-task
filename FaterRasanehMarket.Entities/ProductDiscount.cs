using System;
using System.Collections.Generic;
using System.Text;

namespace FaterRasanehMarket.Entities
{
   public class ProductDiscount
    {
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Discount Discount { get; set; }
    }
}
