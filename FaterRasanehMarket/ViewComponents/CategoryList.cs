using Microsoft.AspNetCore.Mvc;
using FaterRasanehMarket.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    [ViewComponent(Name = "CategoryList")]
    public class CategoryList : ViewComponent
    {
        private readonly IUnitOfWork _uw;
        public CategoryList(IUnitOfWork uw)
        {
            _uw = uw;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool MainList=false)
        {
            if(MainList==true)
                return View("CategoryMainList",await _uw.CategoryRepository.GetAllCategoriesAsync());
            return View(await _uw.CategoryRepository.GetAllCategoriesAsync());
        }
    }
}
