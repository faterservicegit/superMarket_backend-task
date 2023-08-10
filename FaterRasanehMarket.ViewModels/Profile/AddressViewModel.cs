using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Profile
{
   public class AddressViewModel
    {
        public int? Id { get; set; }
        [Display(Name = "عنوان"), JsonPropertyName("عنوان")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Title { get; set; }
        [Display(Name = "آدرس"), JsonPropertyName("آدرس")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string FullAddress { get; set; }
        [Display(Name = "پیشفرض"), JsonPropertyName("پیشفرض")]
        public bool? IsDefualt { get; set; }
        [Display(Name = "شماره تماس"), JsonPropertyName("شماره تماس")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PhoneNumber { get; set; }
    }
}
