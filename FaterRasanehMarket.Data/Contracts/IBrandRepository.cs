using FaterRasanehMarket.ViewModels.Brand;
using FaterRasanehMarket.ViewModels.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface IBrandRepository
    {
        Task<int> CountBrandProducts(int BrandId);
        Task<string> CheckBrandImage(string ImageName);
        Task<List<BrandViewModel>> GetPaginateBrandsAsync(int offset, int limit, string orderBy, string searchText);
        Task<List<ProductViewModel>> GetPaginateBrandProductsAsync(int offset, int limit, int brandId);

    }
}
