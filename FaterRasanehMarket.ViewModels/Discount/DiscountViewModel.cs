using FaterRasanehMarket.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Discount
{
   public class DiscountViewModel
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        [Display(Name = "درصد تخفیف"), JsonPropertyName("درصد"),Range(1, 100, ErrorMessage = "درصد تخفیف صحیح نمی باشد")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public int Percent { get; set; }
        [JsonIgnore]
        public bool IsPublic { get; set; }
        [JsonPropertyName("IsExpired")]
        public bool IsExpired { get; set; }
        [JsonPropertyName("IsActivated")]
        public bool IsActivated { get; set; }
        [Display(Name = "کد تخفیف"), JsonPropertyName("کد تخفیف")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Code { get; set; }
        [Display(Name = "تاریخ شروع تخفیف"), JsonPropertyName("تاریخ شروع")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public string PersianStartDate { get; set; }
        [JsonIgnore]
        public DateTime StartDate { get; set; }
        [Display(Name = "تاریخ انقضا تخفیف"), JsonPropertyName("تاریخ انقضا")]
        public string PersianEndDate { get; set; }
        [JsonIgnore]
        public DateTime? EndDate { get; set; }
        [JsonPropertyName("ProductsCount")]
        public int ProductsCount { get; set; }
        [JsonPropertyName("OrdersCount")]
        public int OrdersCount { get; set; }
        [JsonIgnore,Display(Name ="بدون تاریخ انقضا")]
        public bool IsInfiniteDiscount { get; set; } = true;
        [JsonIgnore, Display(Name = "کالاهای شامل تخفیف")]
        public string[] ProductIds { get; set; }
        [JsonIgnore]
        public IEnumerable<ProductDiscount> ProductsDiscount { get; set; }
    }
}
