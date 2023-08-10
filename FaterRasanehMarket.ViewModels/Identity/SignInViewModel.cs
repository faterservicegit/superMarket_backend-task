
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FaterRasanehMarket.ViewModels.Identity
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Display(Name = "مرا به خاطر بسپار؟")]
        public bool RememberMe { get; set; }
        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [JsonIgnore]
        public string ReturnUrl { get; set; }
    }
}
