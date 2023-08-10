using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.ViewComponents
{
    public class RelatedProducts : ViewComponent
    {
        private readonly IUnitOfWork _UW;
        public RelatedProducts(IUnitOfWork UW)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        public async Task<IViewComponentResult> InvokeAsync(int ProductId)
        {
            var currentCategories = await _UW._Context.ProductCategories.Where(t => t.ProductId == ProductId).Select(t => t.CategoryId).ToListAsync();
            var products = await _UW._Context.ProductCategories.Where(t=>currentCategories.Contains(t.CategoryId)&t.ProductId!=ProductId).Include(t => t.Product).ThenInclude(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                .Where(t=>t.Product.IsDeleted==false&t.Product.Count>0)
                .Select(t => new ProductViewModel
                {
                    Id = t.Product.Id,
                    Name = t.Product.Name,
                    Icon = t.Product.Icon,
                    Price=t.Product.Price,
                    LikePrecent = t.Product.Likes.Count > 0 ? t.Product.Likes.Sum(t => t.LikeNumber) / (t.Product.Likes.Count()) : 0,
                    Count = t.Product.ProductOrders.Sum(t => t.Count),
                    DiscountPrecent = t.Product.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (int?)t.Product.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Sum(t=>t.Percent): null,
                    DiscountedAmount = t.Product.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (int?)(t.Product.Price - (t.Product.Price * (t.Product.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Sum(t => t.Percent))) / 100) : null,
                }).Distinct().OrderByDescending(t => Guid.NewGuid()).Take(10).ToListAsync();
            return View(products);
        }
    }
}
