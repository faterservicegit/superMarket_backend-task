using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Category;
using FaterRasanehMarket.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "مدیریت,فروشنده")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UW;
        private const string CategoryNotFound = "دسته ی درخواستی یافت نشد.";
        private const string CategoryDuplicate = "نام دسته تکراری است.";
        private readonly IMapper _mapper;
        public CategoryController(IUnitOfWork UW, IMapper mapper)
        {
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> GetCategories(string search, string order, int offset, int limit, string sort)
        {
            List<CategoryViewModel> categories;
            int total = _UW.BaseRepository<Category>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "دسته")
            {
                if (order == "asc")
                    categories = await _UW.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "CategoryInfo.Name", search);
                else
                    categories = await _UW.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "CategoryInfo.Name desc", search);
            }

            else if (sort == "دسته پدر")
            {
                if (order == "asc")
                    categories = await _UW.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "ParentInfo.Name", search);
                else
                    categories = await _UW.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "ParentInfo.Name desc", search);
            }
            else
                categories = await _UW.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, "CategoryInfo.Name", search);

            if (search != "")
                total = categories.Count();

            return Json(new { total, rows = categories });
        }

        [HttpGet, AjaxOnly, DisplayName("درج و ویرایش")]
        public async Task<IActionResult> RenderCategory(int? categoryId)
        {
            var categoryViewModel = new CategoryViewModel();
            ViewBag.Categories = await _UW.CategoryRepository.GetAllCategoriesAsync();
            if (categoryId != null)
            {
                var category = await _UW.BaseRepository<Category>().FindByIdAsync(categoryId);
                _UW._Context.Entry(category).Reference(c => c.Parent).Load();
                if (category != null)
                    categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                else
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
            }
            return PartialView("_RenderCategory", categoryViewModel);
        }

        [HttpPost, AjaxOnly]
        public async Task<IActionResult> CreateOrUpdate(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_UW.CategoryRepository.IsExistCategory(viewModel.Name, viewModel.Id))
                    ModelState.AddModelError(string.Empty, CategoryDuplicate);
                else
                {
                    if (viewModel.ParentName.HasValue())
                    {
                        var parentCategory = await _UW.CategoryRepository.FindByNameAsync(viewModel.ParentName);
                        if (parentCategory != null)
                            viewModel.ParentCategoryId = parentCategory.Id;
                        else
                        {
                            Category parent = new Category()
                            {
                                Name = viewModel.ParentName,
                            };
                            await _UW.BaseRepository<Category>().CreateAsync(parent);
                            await _UW.Commit();
                            viewModel.ParentCategoryId = parent.Id;
                        }
                    }

                    if (viewModel.Id != 0)
                    {
                        var category = await _UW.BaseRepository<Category>().FindByIdAsync(viewModel.Id);
                        if (category != null)
                        {
                            _UW.BaseRepository<Category>().Update(_mapper.Map(viewModel, category));
                            await _UW.Commit();
                            TempData["notification"] = "ویرایش دسته بندی با موفقیت انجام شد";
                        }
                        else
                            ModelState.AddModelError(string.Empty, CategoryNotFound);
                    }
                    else
                    {
                        await _UW.BaseRepository<Category>().CreateAsync(_mapper.Map<Category>(viewModel));
                        await _UW.Commit();
                        TempData["notification"] = "درج دسته بندی با موفقیت انجام شد";
                    }
                }
            }

            return PartialView("_RenderCategory", viewModel);
        }


        [HttpGet, AjaxOnly, DisplayName("حذف")]
        public async Task<IActionResult> Delete(int? categoryId)
        {
            if (categoryId==null)
                ModelState.AddModelError(string.Empty, CategoryNotFound);
            else
            {
                var category = await _UW.BaseRepository<Category>().FindByIdAsync(categoryId);
                if (category == null)
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
                else
                    return PartialView("_DeleteConfirmation", category);
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Category model)
        {
            if (model.Id == 0)
                ModelState.AddModelError(string.Empty, CategoryNotFound);
            else
            {
                var category = await _UW.BaseRepository<Category>().FindByIdAsync(model.Id);

                if (category == null)
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
                else
                {
                    var childCategory = _UW.BaseRepository<Category>().FindByConditionAsync(c => c.ParentCategoryId == category.Id).Result.ToList();
                    if (childCategory.Count() != 0)
                    {
                        _UW.BaseRepository<Category>().DeleteRange(childCategory);
                    }
                    _UW.BaseRepository<Category>().Delete(category);
                    await _UW.Commit();
                    TempData["notification"] = "حذف دسته بندی با موفقیت انجام شد";
                    return PartialView("_DeleteConfirmation", category);
                }
            }
            return PartialView("_DeleteConfirmation");
        }
        [HttpPost, ActionName("DeleteGroup"), AjaxOnly, DisplayName("حذف گروهی")]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ دسته بندی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var childCategory = _UW.BaseRepository<Category>().FindByConditionAsync(c => c.ParentCategoryId == int.Parse(item)).Result.ToList();
                    if (childCategory.Count() != 0)
                    {
                        _UW.BaseRepository<Category>().DeleteRange(childCategory);
                    }
                    var category = await _UW.BaseRepository<Category>().FindByIdAsync(int.Parse(item));
                    _UW.BaseRepository<Category>().Delete(category);
                }
                await _UW.Commit();
                TempData["notification"] = "حذف گروهی اطلاعات با موفقیت انجام شد.";
            }
            return PartialView("_DeleteGroup");
        }
    }
}
