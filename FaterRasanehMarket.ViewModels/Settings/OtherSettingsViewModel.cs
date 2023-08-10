using System.ComponentModel.DataAnnotations;

namespace FaterRasanehMarket.ViewModels.Settings
{
    public class OtherSettingsViewModel
    {
        public EmailSettingsViewModel EmailSettings { get; set; }
        public OrderSettingsViewModel OrderSettings { get; set; }
    }
    public class OrderSettingsViewModel
    {
        [Display(Name = "سفارش گیری")]
        public bool IsTakingOrder { get; set; }
        [Display(Name = "پرداخت در محل")]
        public bool OfflinePayment { get; set; }
        [Display(Name = "پرداخت آنلاین")]
        public bool OnlinePayment { get; set; }
        [Display(Name = "هزینه ارسال")]
        public int ShippingCost { get; set; }
        [Display(Name = "حداقل سفارش برای ارسال رایگان")]
        public int MinOrderForFreeShipping { get; set; }
    }
    public class EmailSettingsViewModel
    {
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }
        [Display(Name = "هاست")]
        public string Host { get; set; }
        [Display(Name = "پورت")]
        public int Port { get; set; }
    }
}
