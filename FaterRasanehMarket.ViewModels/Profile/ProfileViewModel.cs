using FaterRasanehMarket.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FaterRasanehMarket.ViewModels.Profile
{
    public class ProfileViewModel
    {
        [Display(Name ="نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string LastName { get; set; }
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }
        [Display(Name = "شماره کارت بانکی")]
        public string CreditCardNumber { get; set; }
        [StringLength(30, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Password), Display(Name = "کلمه عبور جدید")]
        public string OldPassword { get; set; }
        [StringLength(30, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Password), Display(Name = "کلمه عبور جدید")]
        public string Password { get; set; }
        [DataType(DataType.Password), Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }
        public List<Address> Addresses { get; set; }
        public bool PasswordProblem { get; set; } = false;
    }
}
