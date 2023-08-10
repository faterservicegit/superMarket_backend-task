using FaterRasanehMarket.Common;
using System.ComponentModel.DataAnnotations;

namespace FaterRasanehMarket.ViewModels.CartAndOrders
{
    public class CheckOutViewModel
    {
        public int? AddressId { get; set; }
        [Display(Name = "کد تخفیف")]
        public string DiscountCode { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        public OrderPaymentMethod OrderPaymentMethod { get; set; }
        public CartViewModel Cart { get; set; } = new CartViewModel();
    }
}
