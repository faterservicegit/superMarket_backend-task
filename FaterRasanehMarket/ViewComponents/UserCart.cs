using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.CartAndOrders;
using FaterRasanehMarket.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    public class UserCart : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        public UserCart(IUnitOfWork UW, IWritableOptions<SiteSettings> writableLocations)
        {
            _writableLocations = writableLocations;
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = new CartViewModel();
            var userId = int.Parse(User.Identity.GetUserId());
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(userId);
            if (order != null)
            {
                cart.ShippingCost = _writableLocations.Value.OrderSettings.MinOrderForFreeShipping <= order.OrderProducts.Sum(t => t.Price * t.Count) || _writableLocations.Value.OrderSettings.MinOrderForFreeShipping == 0 ? 0 : _writableLocations.Value.OrderSettings.ShippingCost;
                cart.OrderPrice = order.OrderProducts.Sum(t => t.Price * t.Count);
                cart.OrderDetails = order.OrderProducts;
                cart.TotalPrice =  cart.ShippingCost + cart.OrderPrice;
            }
            return View(cart);
        }
    }
}
