using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Discount;
using FaterRasanehMarket.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Repositories
{
    public class DiscountRepository: IDiscountRepository
    {
        private readonly FaterRasanehMarketDBContext _Context;
        public DiscountRepository(FaterRasanehMarketDBContext Context)
        {
            _Context = Context;
            _Context.CheckArgumentIsNull(nameof(_Context));
        }

        public async Task<List<DiscountViewModel>> GetPaginateDiscountsAsync(int offset, int limit, string orderBy, string searchText, bool IsPublic)
        {
                var DiscountViewModel = await  _Context.Discounts.Where(n => n.IsPublic == IsPublic &( n.Code.Contains(searchText) || n.Percent.ToString().Contains(searchText)||n.Id.ToString().Contains(searchText)))
                              .Include(t => t.ProductsDiscount).ThenInclude(t=>t.Product).Include(t=>t.Orders)
                    .Select(t=>new DiscountViewModel
                    {
                        Id = t.Id,
                        Percent = t.Percent,
                        Code = t.Code,
                        IsPublic = t.IsPublic,
                        IsExpired =t.EndDate < DateTime.Now,
                        ProductsCount = t.ProductsDiscount.Count(t=>t.Product.IsDeleted==false),
                        StartDate=t.StartDate,
                        EndDate=t.EndDate,
                        OrdersCount=t.Orders.Count(),
                        IsActivated = (t.ProductsDiscount.Count != t.ProductsDiscount.Select(t => t.Product).Count(t => t.IsDeleted == true)||t.IsPublic==true) && t.StartDate < DateTime.Now,
                        PersianStartDate = ((DateTime?)t.StartDate).ConvertMiladiToShamsi("yyyy/MM/dd"),
                        PersianEndDate = t.EndDate == null ? "نامحدود" : t.EndDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                    }).AsNoTracking().OrderBy(orderBy).Skip(offset).Take(limit).AsNoTracking().ToListAsync();
                int row = 0;
                foreach (var item in DiscountViewModel)
                {
                    item.Row = ++row;
                }
                return DiscountViewModel;
        }
        public async Task<List<ProductViewModel>> GetDiscountProductsAsync(int discountId)
        {
            return await _Context.ProductDiscounts.Include(t => t.Product).Where(t => t.DiscountId == discountId).Include(t=>t.Discount)
                .Select(t => new ProductViewModel {
                Id=t.ProductId,
                Name=t.Product.Name,
                Price=t.Product.Price,
                Count=t.Product.Count,
                IsDeleted=t.Product.IsDeleted,
                DiscountedAmount=t.Product.Price-(t.Product.Price*t.Discount.Percent/100),
                }).AsNoTracking().ToListAsync();
        }
        public async Task<Discount> GetDiscountWithCodeAsync(string Code)
        {
            return await _Context.Discounts.Where(t => t.Code == Code).Include(t=>t.ProductsDiscount).Include(t=>t.Orders).ThenInclude(t=>t.Customer).AsNoTracking().FirstOrDefaultAsync();
        }

    }
}
