using FaterRasanehMarket.Common;
using FaterRasanehMarket.Common.Attributes;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.Services.Hubs;
using FaterRasanehMarket.ViewModels.CartAndOrders;
using FaterRasanehMarket.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Controllers
{
    public class CartAndOrderController : Controller
    {
        private readonly IUnitOfWork _UW;
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        private readonly IHubContext<SellerHub> _sellerHub;

        public CartAndOrderController(IUnitOfWork UW, IWritableOptions<SiteSettings> writableLocations, IHubContext<SellerHub> sellerHub)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
            _writableLocations = writableLocations;
            _sellerHub = sellerHub;
            _sellerHub.CheckArgumentIsNull(nameof(_sellerHub));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var orders = (await _UW.BaseRepository<Order>().FindByConditionAsync(t => t.CustomerId == int.Parse(User.Identity.GetUserId()) & t.OrderStatus != OrderStatusCode.AwaitingPayment)).OrderByDescending(t => t.PaidDateTime).ToList();
            return View(orders);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Factor(string orderId, bool IsPanel = false)
        {
            ViewBag.IsPanel = IsPanel;
            var order = await _UW.CartAndOrdersRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return BadRequest("فاکتور پیدا نشد");
            return PartialView("_Factor", order);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserCart()
        {
            var cart = new CartViewModel();
            var userId = int.Parse(User.Identity.GetUserId());
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(userId);
            if (order != null)
            {
                if (!Request.IsAjaxRequest())
                {

                    var Result = await _UW.CartAndOrdersRepository.CheckForCartUpdate(order);
                    if (Result == true)
                        ViewBag.CartUpdated = "تغییراتی در سبد خرید شما پیش  آمده لطفا قبل از سفارش  سبد خرید خود را بررسی نمایید ";
                }

                cart.ShippingCost = _writableLocations.Value.OrderSettings.MinOrderForFreeShipping <= order.OrderProducts.Sum(t => t.Price * t.Count) || _writableLocations.Value.OrderSettings.MinOrderForFreeShipping == 0 ? 0 : _writableLocations.Value.OrderSettings.ShippingCost;
                cart.OrderPrice = order.OrderProducts.Sum(t => t.Price * t.Count);
                cart.OrderDetails = order.OrderProducts;
                cart.TotalPrice = cart.ShippingCost + cart.OrderPrice;
            }
            if (Request.IsAjaxRequest())
                return ViewComponent("UserCart", cart);
            return View(cart);
        }
        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            if (!User.Identity.IsAuthenticated)
                return BadRequest("برای خرید وارد حساب کاربریتان شوید");
            var userId = int.Parse(User.Identity.GetUserId());
            var product = await _UW.ProductRepository.GetProductCurrentCountAndPriceAsync(productId);
            if (product == null)
                return NotFound("محصول موردنظر پیدا نشد");
            if (product.Count < count)
                return BadRequest("موجودی محصول کمتر از تعداد انتخابی است");
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(userId);
            if (order == null)
            {
                order = new Order
                {
                    Id = await _UW.CartAndOrdersRepository.GenerateOrderIdAsync(),
                    CustomerId = userId,
                    OrderStatus = OrderStatusCode.AwaitingPayment,
                };
                await _UW.BaseRepository<Order>().CreateAsync(order);
                await _UW.Commit();
            }
            var detail = order.OrderProducts?.Where(t => t.ProductId == productId).FirstOrDefault();
            if (detail == null)
            {
                await _UW.BaseRepository<OrderDetail>().CreateAsync(new OrderDetail
                {
                    ProductId = productId,
                    OrderId = order.Id,
                    Price = product.DiscountedAmount != null ? (int)product.DiscountedAmount : product.Price,
                    Count = count,
                });
            }
            else
            {
                if (detail.Count + count > product.Count)
                    return BadRequest("موجودی محصول کمتر از تعداد انتخابی است");
                detail.Count += count;
            }
            await _UW.Commit();
            return Ok("محصول با موفقیت به سبد خرید اضافه شد");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveOrUpdateFromCart(int productId, int? count)
        {
            var userId = int.Parse(User.Identity.GetUserId());
            var product = await _UW.ProductRepository.GetProductCurrentCountAndPriceAsync(productId);
            if (product == null)
                return NotFound("کالای موردنظر پیدا نشد");
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(userId);
            var detail = order.OrderProducts?.Where(t => t.ProductId == productId).FirstOrDefault();
            if (detail == null)
                return Ok("محصول موردنظر از سبد خرید شما حدف شده است");
            else
            {
                if (count == null)
                {
                    _UW.BaseRepository<OrderDetail>().Delete(detail);
                    if (order.OrderProducts.Count <= 1)
                        _UW.BaseRepository<Order>().Delete(order);
                    await _UW.Commit();
                    return Ok("محصول موردنظر با موفقیت از سبد خرید حذف شد");
                }
                else
                {
                    if (count > product.Count)
                        return BadRequest("موجودی محصول کمتر از تعداد انتخابی است");
                    else
                    {
                        detail.Count = (int)count;
                        await _UW.Commit();
                        return Ok("سبد خرید بروزرسانی شد");
                    }
                }
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            var viewModel = new CheckOutViewModel();
            var userId = int.Parse(User.Identity.GetUserId());
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(userId);
            if (order == null || order!.OrderProducts.Count == 0)
                return RedirectToAction("Index", "Home");
            var Result = await _UW.CartAndOrdersRepository.CheckForCartUpdate(order);
            if (Result == true)
                ViewBag.CartUpdated = "تغییراتی در سبد خرید شما پیش  آمده لطفا قبل از سفارش  سبد خرید خود را بررسی نمایید ";
            viewModel.Cart.ShippingCost = _writableLocations.Value.OrderSettings.MinOrderForFreeShipping <= order.OrderProducts.Sum(t => t.Price * t.Count) || _writableLocations.Value.OrderSettings.MinOrderForFreeShipping == 0 ? 0 : _writableLocations.Value.OrderSettings.ShippingCost;
            viewModel.Cart.OrderPrice = order.OrderProducts.Sum(t => t.Price * t.Count);
            viewModel.Cart.OrderDetails = order.OrderProducts;
            viewModel.Cart.TotalPrice = viewModel.Cart.ShippingCost + viewModel.Cart.OrderPrice;
            viewModel.DiscountCode = order.Discount?.Code;
            viewModel.OrderPaymentMethod = order.PaymentMethod;
            viewModel.Cart.DiscountAmount = order.Discount != null ? viewModel.Cart.OrderPrice * order.Discount.Percent / 100 : 0;
            viewModel.AddressId = order.AddressId;
            order.ShippingCost = _writableLocations.Value.OrderSettings.MinOrderForFreeShipping <= order.OrderProducts.Sum(t => t.Price * t.Count) || _writableLocations.Value.OrderSettings.MinOrderForFreeShipping == 0 ? 0 : _writableLocations.Value.OrderSettings.ShippingCost;
            order.AmountPaid = viewModel.Cart.OrderPrice - viewModel.Cart.DiscountAmount;
            await _UW.Commit();
            return View(viewModel);
        }
        [Authorize]
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> AddOrRemoveDiscountFromCart(string Code)
        {
            var userId = int.Parse(User.Identity.GetUserId());
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(userId);
            if (order.DiscountId != null)
            {
                order.DiscountId = null;
                await _UW.Commit();
                return Ok("کد تخفیف با موفقیت حذف شد");
            }
            var discount = await _UW.DiscountRepository.GetDiscountWithCodeAsync(Code);
            if (discount == null)
                return BadRequest("کد تخفیف نا معتبر است");
            if (discount.StartDate > DateTime.Now)
                return BadRequest("کد تخفیف درحال حاظر فعال نیست");
            if (discount.EndDate != null & discount.EndDate < DateTime.Now)
                return BadRequest("کد تخفیف منقضی شده است");
            if (discount.Orders.Select(t => t.CustomerId).Contains(userId))
                return BadRequest("کد تخفیف قبلا استفاده شده است");
            order.DiscountId = discount.Id;
            await _UW.Commit();
            return Ok("کد تخفیف با موفقیت اعمال شد");
        }
        [HttpPost]
        [ActionName("CheckOut")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder(CheckOutViewModel viewModel)
        {
            var order = await _UW.CartAndOrdersRepository.GetUserCartAsync(int.Parse(User.Identity.GetUserId()));
            if (ModelState.IsValid)
            {
                var result = await _UW.CartAndOrdersRepository.CheckForCartUpdate(order);
                if (result == true)
                    ModelState.AddModelError("", "تغییراتی در سبد خرید شما رخ داده است لطفا قبل از تایید سفارش مجددا چک فرمایید");
                else
                {
                    order.PaymentMethod = viewModel.OrderPaymentMethod;
                    order.AddressId = viewModel.AddressId;
                    order.Description = viewModel.Description;
                    if (viewModel.OrderPaymentMethod == OrderPaymentMethod.OfflinePayment)
                    {
                        order.OrderStatus = OrderStatusCode.Waitingforapproval;
                        order.PaidDateTime = DateTime.Now;
                        foreach (var item in order.OrderProducts)
                        {
                            item.Product.Count = item.Product.Count - item.Count;
                            if (item.Product.Count <= item.Product.CountLimit)
                            {
                                item.Product.LimitEnabledDateTime = DateTime.Now;
                                await _sellerHub.Clients.All.SendAsync("alertWarningMessage", $"موجودی کالای {item.Product.Name} (کد کالا : {item.Product.Id}) در حال اتمام است.");
                            }
                        }
                        await _UW.Commit();
                        var customer = $"{order.Customer.FirstName} {order.Customer.LastName} ({order.Customer.UserName})";
                        await _sellerHub.Clients.All.SendAsync("HandleNewOrder",new {order.Id,Amount=order.AmountPaid.ToString("N0"),customer});
                        TempData["notification"] = "خرید شما با موفقیت ثبت شد مراحل سفارش گیری را می توانید در اینجا مشاهده کنید";
                        return RedirectToAction("OrderHistory");
                    }
                    else
                    {
                        order.OrderStatus = OrderStatusCode.AwaitingPayment;
                        await _UW.Commit();
                        //send to pay order
                    }
                }
            }
            viewModel.Cart.ShippingCost = order.ShippingCost;
            viewModel.Cart.OrderPrice = order.OrderProducts.Sum(t => t.Price * t.Count);
            viewModel.Cart.OrderDetails = order.OrderProducts;
            viewModel.Cart.TotalPrice = viewModel.Cart.ShippingCost + viewModel.Cart.OrderPrice;
            viewModel.DiscountCode = order.Discount?.Code;
            viewModel.Cart.DiscountAmount = order.Discount != null ? viewModel.Cart.OrderPrice * order.Discount.Percent / 100 : 0;
            viewModel.AddressId = order.AddressId;
            return View(viewModel);
        }
    }
}