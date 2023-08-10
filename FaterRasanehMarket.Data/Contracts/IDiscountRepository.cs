using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Discount;
using FaterRasanehMarket.ViewModels.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface IDiscountRepository
    {
        Task<List<DiscountViewModel>> GetPaginateDiscountsAsync(int offset, int limit, string orderBy, string searchText, bool IsPublic);
        Task<List<ProductViewModel>> GetDiscountProductsAsync(int discountId);
        Task<Discount> GetDiscountWithCodeAsync(string Code);
    }
}
