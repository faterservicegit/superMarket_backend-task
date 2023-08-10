using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    public class MostNewProducts : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public MostNewProducts(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _UW.ProductRepository.GetPaginateProductsAsync(0, 10, "AddDateTime desc", "", 0,true);
            return View(products);
        }
    }
}
