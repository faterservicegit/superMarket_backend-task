using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Common.Attributes;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.Services;
using FaterRasanehMarket.Services.Contracts;
using FaterRasanehMarket.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UW;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        public OrderController(IUnitOfWork UW, IMapper mapper,IEmailSender emailSender)
        {
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var newOrders = await _UW.CartAndOrdersRepository.CheckNewOrderCountAsync();
            if (newOrders > 0)
                ViewBag.NewOrdersAlert = $"{newOrders} سفارش جدید برای تایید  و ارسال دارید منتظر چی هستید؟!!!";
            return View();
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> GetOrders(string search, string order, string sort, int offset, int limit, OrderStatusCode orderStatus, int? userId)
        {
            List<OrderViewModel> orders;
            int total = await _UW._Context.Orders.CountAsync(t => (orderStatus == 0 || t.OrderStatus == orderStatus) & t.OrderStatus != OrderStatusCode.AwaitingPayment);
            if (!search.HasValue())
                search = "";
            if (limit == 0)
                limit = total;
            if (sort == "شماره سفارش")
            {
                if (order == "asc")
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "Id", search, orderStatus, userId);
                else
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "Id desc", search, orderStatus, userId);
            }
            else if (sort == "زمان ثبت")
            {
                if (order == "asc")
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "PaidDateTime", search, orderStatus, userId);
                else
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "PaidDateTime desc", search, orderStatus, userId);
            }
            else if (sort == "زمان ارسال")
            {
                if (order == "asc")
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "ShipDateTime", search, orderStatus, userId);
                else
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "ShipDateTime desc", search, orderStatus, userId);
            }
            else if (sort == "مبلغ")
            {
                if (order == "asc")
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "AmountPaid", search, orderStatus, userId);
                else
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "AmountPaid desc", search, orderStatus, userId);
            }
            else if (sort == "روش پرداخت")
            {
                if (order == "asc")
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "PaymentMethod", search, orderStatus, userId);
                else
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "PaymentMethod desc", search, orderStatus, userId);
            }
            else if (sort == "وضعیت")
            {
                if (order == "asc")
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "OrderStatus", search, orderStatus, userId);
                else
                    orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "OrderStatus desc", search, orderStatus, userId);
            }
            else
                orders = await _UW.CartAndOrdersRepository.GetPaginateOrdersAsync(offset, limit, "PaidDateTime desc", search, orderStatus, userId);
            if (search != "")
                total = orders.Count();
            return Json(new { total, rows = orders });
        }
        [HttpGet, AjaxOnly]
        [DisplayName("حذف")]
        public async Task<IActionResult> Delete(string orderId)
        {
            if (orderId == null)
                ModelState.AddModelError("", "سفارش پیدا نشد");
            else
            {
                var Order = await _UW.BaseRepository<Order>().FindByIdAsync(orderId);
                if (Order == null)
                    ModelState.AddModelError("", "سفارش پیدا نشد");
                else
                    return View("_DeleteConfirmation", Order);
            }
            return View("_DeleteConfirmation");
        }
        [HttpPost, AjaxOnly, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Order model)
        {
            var Order = await _UW.CartAndOrdersRepository.GetOrderWithDetailsAsync(model.Id);
            if (Order != null)
            {
                _UW.BaseRepository<Order>().Delete(Order);
                await _UW.Commit();
                TempData["notification"] = "حذف سفارش با موفقیت انجام شد.";
                return PartialView("_DeleteConfirmation", model);
            }
            else
                ModelState.AddModelError("", "سفارش پیدا نشد");
            return PartialView("_DeleteConfirmation");
        }
        [HttpGet, AjaxOnly]
        public IActionResult DeleteGroup()
        {
            return PartialView("_DeleteGroup");
        }
        [HttpPost, AjaxOnly, ActionName("DeleteGroup")]
        public async Task<IActionResult> DeletedGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError("", "هیچ سفارشی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var Order = await _UW.CartAndOrdersRepository.GetOrderWithDetailsAsync(item);
                    _UW.BaseRepository<Order>().Delete(Order);
                }
                await _UW.Commit();
                TempData["notification"] = "حذف گروهی با موفقیت انجام شد.";
            }
            return PartialView("_DeleteGroup");
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> ChangeOrderStatus(string orderId, OrderStatusCode orderStatus)
        {
            var order = await _UW.CartAndOrdersRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return BadRequest();
            if (order.ShipDateTime == null)
                return BadRequest("NeedToSetShippingTime");
            order.OrderStatus = orderStatus;
            await _UW.Commit();
            return Ok("وضعیت سفارش با موفقیت بروز شد");
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> RenderOrder(string orderId, OrderStatusCode? orderStatus)
        {
            var order = await _UW.CartAndOrdersRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return BadRequest();
            var viewModel = new RenderOrderViewModel
            {
                Id = order.Id,
                OrderStatusCode = orderStatus,
                PersianShippingTime = order.ShipDateTime != null ? order.ShipDateTime.ConvertMiladiToShamsi("HH:mm").Fa2En() : ((DateTime?)DateTime.Now.AddMinutes(30)).ConvertMiladiToShamsi("HH:mm").Fa2En(),
                PersianShippingDate = order.ShipDateTime != null ? order.ShipDateTime.ConvertMiladiToShamsi("yyyy/MM/dd").Fa2En() : ((DateTime?)DateTime.Now).ConvertMiladiToShamsi("yyyy/MM/dd").Fa2En(),
            };
            return PartialView("_RenderOrder", viewModel);
        }
        [HttpPost, AjaxOnly]
        public async Task<IActionResult> RenderOrder(RenderOrderViewModel viewModel)
        {
            viewModel.PersianShippingDate = viewModel.PersianShippingDate.Fa2En();
            viewModel.PersianShippingTime = viewModel.PersianShippingTime.Fa2En();
            var order = await _UW.CartAndOrdersRepository.GetOrderWithDetailsAsync(viewModel.Id);
            if (order == null)
                return BadRequest();
            var user = await _UW.BaseRepository<User>().FindByIdAsync(order.CustomerId);
            var persianTimeArray = viewModel.PersianShippingTime.Split(':');
            order.ShipDateTime = viewModel.PersianShippingDate.ConvertShamsiToMiladi().Date + new TimeSpan(int.Parse(persianTimeArray[0]), int.Parse(persianTimeArray[1]), 0);
            if (viewModel.OrderStatusCode != null)
                order.OrderStatus = (OrderStatusCode)viewModel.OrderStatusCode;
            await _UW.Commit();
            if (order.OrderStatus != OrderStatusCode.Waitingforapproval & user.Email != null)
            {
                ViewData["Url"] = $"{Request.Scheme}://{Request.Host}";
                var html = await this.RenderViewAsync("_EmailFactor", order, true);
                Task.Run(() => _emailSender.SendEmailAsync(user.Email, "فاکتور خرید", html));
            }
            TempData["notification"] = "زمان ارسال سفارش با موفقیت ثبت شد";
            return PartialView("_RenderOrder", viewModel);
        }
    }
}
