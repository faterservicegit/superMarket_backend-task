using FaterRasanehMarket.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Brand
{
    public class BrandViewModel
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        [Display(Name = "نام برند"), JsonPropertyName("نام برند")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Name { get; set; }
        [Display(Name = "لوگو برند"), JsonPropertyName("Logo")]
        public string Image { get; set; }
        [Display(Name = "توضیحات"), JsonPropertyName("توضیحات")]
        public string Description { get; set; }

        [JsonIgnore, Display(Name = "عکس")]
        public IFormFile ImageFile { get; set; }
        [JsonPropertyName("تعداد محصولات"), Display(Name = "تعداد محصولات")]
        public int ProductsNumber { get; set; }
    }
}
