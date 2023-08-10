using FaterRasanehMarket.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Product
{
    public class ProductViewModel
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        [JsonPropertyName("Icon"), Display(Name = "آیکون")]
        public string Icon { get; set; }

        [JsonPropertyName("نام"), Display(Name = "نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Name { get; set; }

        [JsonPropertyName("توضیحات"), Display(Name = "توضیحات")]
        public string Description { get; set; }

        [JsonIgnore, Display(Name = "برند")]
        public int? BrandId { get; set; }
        [JsonPropertyName("برند"), Display(Name = "برند")]
        public string BrandName { get; set; }
        [JsonPropertyName("موجودی"), Display(Name = "موجودی")]
        public int Count { get; set; }
        [JsonPropertyName("هشدار موجودی"), Display(Name = "هشدار موجودی")]
        public int CountLimit { get; set; }
        [JsonPropertyName("Price"), Display(Name = "قیمت(ریال)")]
        public int Price { get; set; }
        [JsonPropertyName("DiscountedAmount")]
        public int? DiscountedAmount { get; set; }
        [JsonIgnore]
        public string IdOfCategories { get; set; }
        [JsonPropertyName("تخفیف"), Display(Name = "تخفیف")]
        public int? DiscountPrecent { get; set; }
        [JsonIgnore]
        public bool IsInfiniteDiscount { get; set; } = true;
        [Display(Name = "تاریخ شروع تخفیف"), JsonPropertyName("تاریخ شروع تخفیف")]
        public string DiscountStartDateTime { get; set; }
        [Display(Name = "تاریخ انقضا تخفیف"), JsonPropertyName("تاریخ انقضا تخفیف")]
        public string DiscountEndDateTime { get; set; }
        [JsonPropertyName("دسته بندی")]
        public string NameOfCategories { get; set; }
        [JsonIgnore]
        public ProductCategoriesViewModel ProductCategoriesViewModel { get; set; }
        [JsonIgnore]
        public string[] CategoryIds { get; set; }
        [JsonPropertyName("VisitCount")]
        public int VisitsCount { get; set; }

        [JsonPropertyName("LikesPrecent")]
        public int LikePrecent { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore, Display(Name = "عکس کالا")]
        public virtual List<string> ImagesName { get; set; } = new List<string>();
        [Display(Name = "عکس کالا"),JsonIgnore]
        public List<IFormFile> ImageFiles { get; set; }
        [Display(Name = "آیکون کالا"),JsonIgnore]
        //[Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public IFormFile IconFile { get; set; }
    }
}
