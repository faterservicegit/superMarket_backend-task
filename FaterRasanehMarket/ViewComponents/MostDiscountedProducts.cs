using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    public class MostDiscountedProducts:ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public MostDiscountedProducts(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = (await _UW.ProductRepository.GetPaginateProductsAsync(0, 15, "DiscountPrecent desc", "", 0,true)).Where(t=>t.DiscountedAmount!=null).Take(10).ToList();
            return View(products);
        }
    }
}
