using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Services;
using FaterRasanehMarket.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _UW;
        public HomeController(IUnitOfWork UW)
        {
            _UW = UW;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("[action]")]
        public IActionResult AboutUs()
        {
            return View();
        }
        [Route("[action]")]
        public async Task<IActionResult> Search(string searchText)
        {

            var products = new List<ProductViewModel>();
            if (searchText == "پرفروش_ها")
                products = await _UW.ProductRepository.GetPaginateProductsAsync(0, 30, "OrderCount desc", "", 0);
            else if (searchText == "جدیدترین_ها")
                products = await _UW.ProductRepository.GetPaginateProductsAsync(0, 40, "AddDateTime desc", "", 0);
            else if (searchText == "بیشترین_تخفیف_ها")
                products = (await _UW.ProductRepository.GetPaginateProductsAsync(0, 30, "DiscountPrecent desc", "", 0)).Where(t => t.DiscountedAmount != null).ToList();
            else if (searchText == "پیشنهادهای_ویژه")
                products = (await _UW.ProductRepository.GetPaginateProductsAsync(0, 60, "DiscountPrecent desc", "", 0)).Where(t => t.DiscountedAmount != null).OrderByDescending(t => t.VisitsCount).Take(30).ToList();
            else
                products = await _UW.ProductRepository.GetPaginateProductsAsync(0, 20, "VisitCount desc", searchText, 0);
            ViewBag.Search = searchText;
            return View(products);
        }
    }
}
