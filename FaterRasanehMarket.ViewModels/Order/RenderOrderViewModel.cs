using FaterRasanehMarket.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Order
{
  public  class RenderOrderViewModel
    {
        public string Id { get; set; }
        public OrderStatusCode? OrderStatusCode { get; set; }

        [Display(Name = "تاریخ ارسال")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianShippingDate { get; set; }

        [Display(Name = "زمان ارسال")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianShippingTime { get; set; }
    }
}
