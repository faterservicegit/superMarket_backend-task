using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FaterRasanehMarket.ViewModels.Settings
{
    public class SettingsViewModel
    {
        [Display(Name="عنوان سایت")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        public string Title { get; set; }

        [Display(Name = "معرفی سایت")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Description { get; set; }
        [Display(Name = "آدرس فروشگاه")]
        public string Address { get; set; }

        [Display(Name = "شماره تماس")]
        public string PhoneNumber { get; set; }
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "لوگو")]
        public IFormFile Logo{ get; set; }

        [Display(Name = "فاویکن")]
        public IFormFile Favicon { get; set; }
    }
}
