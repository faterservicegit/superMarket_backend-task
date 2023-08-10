using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Controllers
{
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _UW;
        public BrandController(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        [HttpGet]
        public async Task<IActionResult> BrandDetails(int brandId, int offset = 0, int limit = 20)
        {
            var brand = await _UW.BaseRepository<Brand>().FindByIdAsync(brandId);
            if (brand == null)
                return NotFound();

            var products = await _UW.BrandRepository.GetPaginateBrandProductsAsync(offset, limit, brandId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BrandProductsPagination", products);
            }
            ViewBag.Count = await _UW.BrandRepository.CountBrandProducts(brandId);
            ViewBag.Brand = brand;
            return View(products);
        }
    }
}
