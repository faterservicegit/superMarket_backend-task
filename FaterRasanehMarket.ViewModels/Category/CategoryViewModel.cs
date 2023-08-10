using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Category
{
    public class CategoryViewModel
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [Display(Name = "عنوان دسته بندی"), JsonPropertyName("دسته")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Name { get; set; }

        [Display(Name = "دسته پدر"), JsonPropertyName("دسته پدر")]
        public string ParentName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
