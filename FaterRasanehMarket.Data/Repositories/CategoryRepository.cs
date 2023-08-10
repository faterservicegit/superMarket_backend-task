using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Category;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FaterRasanehMarketDBContext _context;
        private readonly IMapper _mapper;
        public CategoryRepository(FaterRasanehMarketDBContext context, IMapper mapper)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        public async Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, string orderBy, string searchText)
        {
            List<CategoryViewModel> categories = await _context.Categories.GroupJoin(_context.Categories,
                (cl => cl.ParentCategoryId),
                (or => or.Id),
                ((cl, or) => new { CategoryInfo = cl, ParentInfo = or }))
                .SelectMany(p => p.ParentInfo.DefaultIfEmpty(), (x, y) => new { x.CategoryInfo, ParentInfo = y })
                .OrderBy(orderBy)
                .Skip(offset).Take(limit)
                .Select(c => new CategoryViewModel { Id = c.CategoryInfo.Id, Name = c.CategoryInfo.Name, ParentCategoryId = c.ParentInfo.Id, ParentName = c.ParentInfo.Name }).AsNoTracking().ToListAsync();

            foreach (var item in categories)
                item.Row = ++offset;

            return categories;
        }


        public async Task<List<TreeViewCategory>> GetAllCategoriesAsync()
        {
            var Categories = await (from c in _context.Categories
                                    where (c.ParentCategoryId == null)
                                    select new TreeViewCategory { id = c.Id, title = c.Name }).AsNoTracking().ToListAsync();
            foreach (var item in Categories)
            {
                BindSubCategories(item);
            }
            return Categories;
        }
        public async Task<List<Category>> GetCategoryTreeAsync(int categoryId)
        {
            var Categories = new List<Category>();
            var Category = await  _context.Categories.Where (c=>c.Id == categoryId).FirstOrDefaultAsync();
            Categories.Add(Category);
            while (Categories.Last() != null)
            {
                var category = await _context.Categories.Where(c=>c.Id == Categories.Last().ParentCategoryId).AsNoTracking().FirstOrDefaultAsync();
                Categories.Add(category);
            }
            Categories.Remove(Categories.Last());
                return Categories.OrderBy(t=>t.Id).ToList();
        }
        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories
                                 where (c.ParentCategoryId == category.id)
                                 select new TreeViewCategory { id = c.Id, title = c.Name }).AsNoTracking().ToList();
            foreach (var item in SubCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }
        }

        public async Task<Category> FindByNameAsync(string categoryName)
        {
            return await _context.Categories.Where(c => c.Name == categoryName.TrimStart().TrimEnd()).FirstOrDefaultAsync();
        }
        public bool IsExistCategory(string categoryName, int recentCategoryId)
        {
            if (recentCategoryId == 0)
                return _context.Categories.Any(c => c.Name.Trim().Replace(" ", "") == categoryName.Trim().Replace(" ", ""));
            else
            {
                var category = _context.Categories.Where(c => c.Name.Trim().Replace(" ", "") == categoryName.Trim().Replace(" ", "")).FirstOrDefault();
                if (category == null)
                    return false;
                else
                {
                    if (category.Id != recentCategoryId)
                        return true;
                    else
                        return false;
                }
            }
        }


    }
}
