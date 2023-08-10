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
    public class UserAddresses:ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public UserAddresses(IUnitOfWork UW)
        {
            _UW = UW;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool IsCheckOut = true)
        {
            if (IsCheckOut == false)
                return View("CategoryMainList", await _UW.CategoryRepository.GetAllCategoriesAsync());
            var adresses = await _UW.BaseRepository<Address>().FindByConditionAsync(t => t.UserId == int.Parse(User.Identity.GetUserId()));
            return View(adresses);
        }
    }
}
