using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.ViewModels.Brand;
using FaterRasanehMarket.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Repositories
{
   public class BrandRepository: IBrandRepository
    {
        private readonly FaterRasanehMarketDBContext _Context;
        public BrandRepository(FaterRasanehMarketDBContext Context)
        {
            _Context = Context;
            _Context.CheckArgumentIsNull(nameof(_Context));
        }
        public async Task<List<BrandViewModel>> GetPaginateBrandsAsync(int offset, int limit, string orderBy, string searchText)
        {
            var BrandViewModel = await _Context.Brands.Where(n => n.Name.Contains(searchText) || n.Description.Contains(searchText)).Include(t=>t.Products)
                            .Select(t => new BrandViewModel
                            {
                                Id = t.Id,
                                Name = t.Name,
                                Description = t.Description,
                                Image = t.Image,
                                ProductsNumber=t.Products.Count()
                            }).AsNoTracking().OrderBy(orderBy).Skip(offset).Take(limit).AsNoTracking().ToListAsync();
            int row = 0;
            foreach (var item in BrandViewModel)
            {
                item.Row = ++row;
            }
            return BrandViewModel;
        }
        public async Task<List<ProductViewModel>> GetPaginateBrandProductsAsync(int offset, int limit,int brandId)
        {
            var products =  await (from s in _Context.Products.Where(n => n.IsDeleted == false&n.BrandId==brandId)
                                        .Include(t => t.Brand).Include(t => t.Likes).Include(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                                        .OrderByDescending(t=>t.AddDateTime)
                                  select (new ProductViewModel
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      Description = s.Description,
                                      ImagesName = s.Images.Select(t => t.Name).ToList(),
                                      Count = s.Count,
                                      Price = s.Price,
                                      Icon = s.Icon,
                                      DiscountPrecent = s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (int?)s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Sum(t => t.Percent) : null,
                                      DiscountedAmount = s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (int?)(s.Price - (s.Price * (s.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Sum(t => t.Percent))) / 100) : null,
                                      LikePrecent = s.Likes.Count > 0 ? s.Likes.Sum(t => t.LikeNumber) / (s.Likes.Count()) : 0,
                                  })).AsNoTracking().Skip(offset).Take(limit).ToListAsync();
            return products;
        }
        public async Task<string> CheckBrandImage(string ImageName)
        {
            string Ex = Path.GetExtension(ImageName);
            int FileCount = await _Context.Brands.Where(v => v.Image == ImageName).CountAsync();
            int j = 1;
            while (FileCount != 0)
            {
                ImageName = ImageName.Replace(Ex, "") + j + Ex;
                FileCount = _Context.Brands.Where(v => v.Image == ImageName).Count();
                j++;
            }
            return ImageName;
        }
        public async Task<int> CountBrandProducts(int BrandId)
        {
            return await _Context.Products.Where(t=>t.BrandId==BrandId&t.IsDeleted==false).AsNoTracking().CountAsync();
        }
    }
}
