using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Discount;
using FaterRasanehMarket.ViewModels.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface IProductRepository
    {
        Task<List<ProductViewModel>> GetPaginateProductsAsync(int offset, int limit, string orderBy, string searchText,int categoryId,bool JustAvailable=false);
        Task<Product> GetProductWithDetails(int? productId);
        Task<string> CheckProductIcon(string IconName);
        Task<string> CheckProductImage(string ImageName);
        Task<List<DiscountViewModel>> GetProductDiscounts(int productId, bool? IsActive);
        Task<ProductViewModel> GetProductCurrentCountAndPriceAsync(int productId);
        Task<int> NonExistentProductsCountAsync(int ExistentHour = 24);
    }
}
