using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace FaterRasanehMarket.Common
{
    public enum OrderStatusCode
    {
        [Display(Name = "درانتظار پرداخت")]
        AwaitingPayment = 1,
        [Display(Name = "در انتظار تایید")]
        Waitingforapproval = 2,
        [Display(Name = "درحال آماده سازی")]
        Preparing = 3,
        [Display(Name = "ارسال شده")]
        Posted = 4,
        [Display(Name = "تحویل داده شد")]
        Delivered = 5,
    }
    public enum OrderPaymentMethod
    {
        [Display(Name = "پرداخت در محل")]
        OfflinePayment = 1,
        [Display(Name = "پرداخت اینترنتی")]
        OnlinePayment = 2,
    }
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}
