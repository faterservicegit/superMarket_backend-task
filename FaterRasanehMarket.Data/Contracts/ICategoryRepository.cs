using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface ICategoryRepository
    {
        Task<Category> FindByNameAsync(string categoryName);
        Task<List<TreeViewCategory>> GetAllCategoriesAsync();
        bool IsExistCategory(string categoryName, int recentCategoryId);
        Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit,string orderBy, string searchText);
        Task<List<Category>> GetCategoryTreeAsync(int categoryId);

    }
}
