using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    public class RandomBrands:ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public RandomBrands(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync(bool All=false)
        {
            var count =All==true?_UW.BaseRepository<Brand>().CountEntities():10;
            if (count == 0)
            count = 1;
            var brands = (await _UW.BrandRepository.GetPaginateBrandsAsync(0, count*2, "ProductsNumber desc", "")).OrderBy(t=>Guid.NewGuid()).Take(count).ToList();
            if(All==true)
                return View("AllBrands",brands);
            return View(brands);
        }
    }
}
