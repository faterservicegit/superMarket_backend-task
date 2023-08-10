using FaterRasanehMarket.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaterRasanehMarket.ViewModels.CartAndOrders
{ 
   public class CartViewModel
    {
        public int ShippingCost { get; set; }
        public int OrderPrice { get; set; }
        public int TotalPrice { get; set; }
        public int DiscountAmount { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
