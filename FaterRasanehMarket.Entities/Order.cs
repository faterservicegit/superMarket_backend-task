using FaterRasanehMarket.Common;
using FaterRasanehMarket.Entities.identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaterRasanehMarket.Entities
{
    public class Order
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public OrderStatusCode OrderStatus { get; set; }
        [ForeignKey("Discount")]
        public int? DiscountId { get; set; }
        public int ShippingCost { get; set; }
        public long AmountPaid { get; set; }
        public string DispatchNumber { get; set; }
        public string Description { get; set; }
        public OrderPaymentMethod PaymentMethod { get; set; }
        public DateTime? PaidDateTime { get; set; }
        public DateTime? ShipDateTime { get; set; }
        public virtual User Customer { get; set; }
        public virtual Address Address { get; set; }
        public virtual Discount Discount { get; set; }
        public virtual List<OrderDetail> OrderProducts { get; set; }
    }
}
