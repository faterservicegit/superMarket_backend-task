using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Identity
{
   public class ChangePasswordViewModel
    {
        public int? Id { get; set; }
        [StringLength(30, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 4)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Password), Display(Name ="کلمه عبور جدید")]
        public string Password { get; set; }
        [DataType(DataType.Password), Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }
    }
}
