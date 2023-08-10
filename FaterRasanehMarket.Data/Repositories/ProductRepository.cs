using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Discount;
using FaterRasanehMarket.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Repositories
{

    public class ProductRepository : IProductRepository
    {
        private readonly FaterRasanehMarketDBContext _Context;
        public ProductRepository(FaterRasanehMarketDBContext Context)
        {
            _Context = Context;
            _Context.CheckArgumentIsNull(nameof(_Context));
        }
        public async Task<string> CheckProductIcon(string IconName)
        {
            string Ex = Path.GetExtension(IconName);
            int FileCount = await _Context.Products.Where(t => t.IsDeleted == false).Where(v => v.Icon == IconName).AsNoTracking().CountAsync();
            int j = 1;
            while (FileCount != 0)
            {
                IconName = IconName.Replace(Ex, "") + j + Ex;
                FileCount = _Context.Products.Where(t => t.IsDeleted == false).Where(v => v.Icon == IconName).Count();
                j++;
            }
            return IconName;
        }
        public async Task<string> CheckProductImage(string ImageName)
        {
            string Ex = Path.GetExtension(ImageName);
            int FileCount = await _Context.Products.Where(t => t.IsDeleted == false).Include(t => t.Images).Where(v => v.Images.Select(t => t.Name).Contains(ImageName)).AsNoTracking().CountAsync();
            int j = 1;
            while (FileCount != 0)
            {
                ImageName = ImageName.Replace(Ex, "") + j + Ex;
                FileCount = await _Context.Products.Where(t => t.IsDeleted == false).Include(t => t.Images).Where(v => v.Images.Select(t => t.Name).Contains(ImageName)).AsNoTracking().CountAsync();
                j++;
            }
            return ImageName;
        }
        public async Task<List<ProductViewModel>> GetPaginateProductsAsync(int offset, int limit, string orderBy, string searchText, int categoryId,bool JustAvailable=false)
        {
            var products = new List<ProductViewModel>();
            var TempData = await (from s in _Context.Products.Include(t => t.ProductCategories).ThenInclude(t => t.Category).Where(n => n.IsDeleted == false&(JustAvailable==true?n.Count>0:true)& (categoryId == 0||categoryId==99999999 || n.ProductCategories.Select(t => t.CategoryId).Contains(categoryId)) & (n.Name.Contains(searchText) || n.Description.Contains(searchText) || n.Id.ToString().Contains(searchText)))
                                        .Include(t => t.Images).Include(t => t.Brand).Include(t => t.Visits).Include(t => t.Likes)
                                        .Include(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                                        .Include(t => t.ProductOrders)
                                  select (new
                                  {
                                      s.Id,
                                      s.Name,
                                      s.Description,
                                      s.Images,
                                      s.Count,
                                      s.Price,
                                      s.Icon,
                                      s.CountLimit,
                                      DiscountPrecent = s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (int?)s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Where(t => t.StartDate < DateTime.Now).Sum(t => t.Percent) : null,
                                      OrderCount = s.ProductOrders.Sum(t=>t.Count),
                                      Discounts = s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)),
                                      BrandName = s.Brand != null ? s.Brand.Name : "ندارد",
                                      s.AddDateTime,
                                      Categories = s.ProductCategories.Select(t => t.Category),
                                      LikesPrecent = s.Likes.Count > 0 ? s.Likes.Sum(t => t.LikeNumber) / (s.Likes.Count()) : 0,
                                      VisitCount = s.Visits.Sum(t => t.NumberOfVisit)
                                  })).Where(t=>categoryId!=99999999 || t.DiscountPrecent!=null).AsNoTracking().OrderBy(orderBy).Skip(offset).Take(limit).ToListAsync();
            int row = 0;
            foreach (var product in TempData)
            {
                string NameOfCategories = "بدون دسته بندی";
                foreach (var category in product.Categories)
                {
                    if (NameOfCategories == "بدون دسته بندی")
                        NameOfCategories = category.Name;
                    else
                    {
                        if (NameOfCategories.Length + category.Name.Length < 40)
                            NameOfCategories = NameOfCategories + " , " + category.Name;
                        else
                            NameOfCategories += ",...";
                    }
                }
                products.Add(new ProductViewModel
                {
                    Row = ++row,
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ImagesName = product.Images.Select(t => t.Name).ToList(),
                    BrandName = product.BrandName,
                    Count = product.Count,
                    CountLimit=product.CountLimit,
                    Price = product.Price,
                    Icon = product.Icon,
                    DiscountPrecent = product.DiscountPrecent,
                    DiscountedAmount = product.Discounts.Count() > 0 ? (int?)(product.Price - (product.Price * (product.Discounts.Sum(t => t.Percent))) / 100) : null,
                    NameOfCategories = NameOfCategories.Length > 43 ? NameOfCategories.Substring(0, 40) + ",..." : NameOfCategories,
                    VisitsCount = product.VisitCount,
                    LikePrecent = product.LikesPrecent
                });
            }
            return products;
        }
        public async Task<Product> GetProductWithDetails(int? productId)
        {
            return await _Context.Products.Where(t => t.Id == productId & t.IsDeleted == false)
                .Include(t => t.Likes).Include(t => t.Images)
                .Include(t => t.ProductCategories).ThenInclude(t=>t.Category)
                .Include(t => t.ProductDiscounts).ThenInclude(t => t.Discount).ThenInclude(t => t.ProductsDiscount)
                .Include(t => t.Brand).FirstOrDefaultAsync();
        }
        public async Task<List<DiscountViewModel>> GetProductDiscounts(int productId, bool? IsActive)
        {
            return await _Context.ProductDiscounts.Where(t => t.ProductId == productId).Include(t => t.Discount)
                .Where(t => t.Discount.IsPublic == false & (IsActive == null || (IsActive == true ? (t.Discount.StartDate < DateTime.Now & (t.Discount.EndDate > DateTime.Now || t.Discount.EndDate == null)) : (t.Discount.StartDate > DateTime.Now || t.Discount.EndDate < DateTime.Now))))
                .Select(t => new DiscountViewModel
                {
                    Id = t.DiscountId,
                    Percent = t.Discount.Percent,
                    PersianStartDate = ((DateTime?)t.Discount.StartDate).ConvertMiladiToShamsi("yyyy/MM/dd"),
                    PersianEndDate = t.Discount.EndDate == null ? "نامحدود" : t.Discount.EndDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                }).AsNoTracking().ToListAsync();
        }
        public async Task<ProductViewModel> GetProductCurrentCountAndPriceAsync(int productId)
        {
            return  await (from s in _Context.Products.Where(n => n.IsDeleted == false & n.Id==productId)
                           .Include(t => t.Brand).Include(t => t.Likes).Include(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                                  select (new ProductViewModel
                                  {
                                      Id = s.Id,
                                      Count = s.Count,
                                      Price = s.Price,
                                      DiscountedAmount = s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (int?)(s.Price - (s.Price * (s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Sum(t => t.Percent))) / 100) : null,
                                  })).AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<int> NonExistentProductsCountAsync(int ExistentHour=24)
        {
            return await  _Context.Products.Where(t => t.IsDeleted == false & t.Count <=t.CountLimit & t.LimitEnabledDateTime > DateTime.Now.AddHours(-1*ExistentHour)).AsNoTracking().CountAsync();
        }
    }
}
