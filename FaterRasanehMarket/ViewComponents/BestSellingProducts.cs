using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    public class BestSellingProducts : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public BestSellingProducts(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync(bool IsMainPage = true)
        {
            var products = await _UW.ProductRepository.GetPaginateProductsAsync(0, 14, "OrderCount desc", "", 0,true);
            if (IsMainPage == true)
                return View(products);
            else
                return View("BestSelling", products.Take(7).ToList());
        }
    }
}
