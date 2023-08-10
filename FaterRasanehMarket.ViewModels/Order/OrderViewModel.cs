using FaterRasanehMarket.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Order
{
   public class OrderViewModel
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        [JsonPropertyName("مشتری")]
        public string Customer { get; set; }
        [JsonPropertyName("مبلغ")]
        public long AmountPaid { get; set; }
        [JsonPropertyName("تخفیف")]
        public long DiscountAmount { get; set; }
        [JsonPropertyName("کد تخفیف استفاده شده")]
        public string DiscountCode { get; set; }
        [JsonPropertyName("هزینه ارسال")]
        public int ShippingCost { get; set; }
        [JsonPropertyName("توضیحات")]
        public string Description { get; set; }
        [JsonPropertyName("روش پرداخت")]
        public string PaymentMethodName { get; set; }
        [JsonPropertyName("وضعیت")]
        public string OrderStatusName { get; set; }
        [JsonPropertyName("OrderStatus")]
        public OrderStatusCode OrderStatus { get; set; }
        [JsonPropertyName("شماره پیگیری پرداخت")]
        public string DispatchNumber { get; set; }

        [JsonIgnore]
        public OrderPaymentMethod PaymentMethod { get; set; }
        [JsonIgnore]
        public DateTime? PaidDateTime { get; set; }
        [JsonIgnore]
        public DateTime? ShipDateTime { get; set; }
        [JsonPropertyName("زمان ثبت")]
        public string PersianPaidDateTime { get; set; }
        [JsonPropertyName("زمان ارسال")]
        public string PersianShipDateTime { get; set; }
    }
}
