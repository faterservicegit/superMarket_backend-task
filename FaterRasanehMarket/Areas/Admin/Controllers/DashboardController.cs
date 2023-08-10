using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.ViewModels;
using MD.PersianDateTime.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "مدیریت,فروشنده")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _UW;
        public DashboardController(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IActionResult> Index()
        {
            var persianDateTime = new PersianDateTime(DateTime.UtcNow.Date);
            var baseTime = persianDateTime.ToShortDateString().ConvertShamsiToMiladi();
            var viewModel = new DashboardViewModel
            {
                UsersCount = _UW.BaseRepository<User>().CountEntities(),
                ProductsCount = await _UW._Context.Products.Where(t => t.IsDeleted == false).CountAsync(),
                VisitsCount = await _UW._Context.Visits.CountAsync(t => t.LastVisitDateTime > baseTime),
                TodaySell = await _UW._Context.Orders.Where(t => t.OrderStatus != OrderStatusCode.AwaitingPayment & t.PaidDateTime > baseTime).SumAsync(t => t.AmountPaid),
            };
            var nonExistentProducts = await _UW.ProductRepository.NonExistentProductsCountAsync();
            if (nonExistentProducts > 0)
                ViewBag.nonExistentProductsAlert = $"{ nonExistentProducts}  محصول در حال اتمام موجودی می باشند.";
            var newOrders = await _UW.CartAndOrdersRepository.CheckNewOrderCountAsync();
            if (newOrders > 0)
                ViewBag.NewOrdersAlert = $"{newOrders} سفارش جدید برای تایید  و ارسال دارید !!!";
            return View(viewModel);
        }
    }
}
