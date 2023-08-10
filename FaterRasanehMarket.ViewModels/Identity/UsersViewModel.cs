using FaterRasanehMarket.Entities;
using FaterRasanehMarket.Entities.identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Identity
{
    public class UsersViewModel
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        [Display(Name="شماره موبایل"), JsonIgnore]
        public string UserName { get; set; }
        [Display(Name = "نام"), JsonPropertyName("نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی"), JsonPropertyName("نام خانوادگی")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string LastName { get; set; }
        [JsonPropertyName("نقش")]
        public string RoleName { get; set; }
        [JsonIgnore]
        [Required(ErrorMessage = "انتخاب {0} الزامی است."), Display(Name = "نقش")]
        public int? RoleId { get; set; }
        [Display(Name = "شماره کارت"), JsonPropertyName("شماره کارت")]
        public string CreditCardNumber { get; set; }
        [Display(Name = "شماره موبایل"), JsonPropertyName("شماره موبایل")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PhoneNumber { get; set; }
        [Display(Name = "تایید شماره موبایل"), JsonPropertyName("PhoneNumberConfimed")]
        public bool PhoneNumberConfirmed { get; set; }
        [Display(Name = "ایمیل"), JsonPropertyName("ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 4)]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Password), Display(Name = "کلمه عبور"),JsonIgnore]
        public string Password { get; set; }
        [DataType(DataType.Password), Display(Name = "تکرار کلمه عبور"),JsonIgnore]
        [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "تاریخ عضویت"),JsonIgnore]
        public DateTime? RegisterDateTime { get; set; }
        [Display(Name = "تاریخ عضویت"), JsonPropertyName("تاریخ عضویت")]
        public string PersianRegisterDateTime { get; set; }
        [JsonIgnore]
        public ICollection< UserRole> Roles { get; set; }
        [JsonIgnore,Display(Name ="نقش")]
        public string[] LimitsId { get; set; }

    }
}
