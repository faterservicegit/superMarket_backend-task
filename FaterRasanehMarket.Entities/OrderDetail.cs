using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FaterRasanehMarket.Entities
{
   public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public string OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
