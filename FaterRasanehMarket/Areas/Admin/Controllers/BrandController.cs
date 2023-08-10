using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Brand;
using FaterRasanehMarket.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "مدیریت,فروشنده")]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _UW;
        private readonly IWebHostEnvironment _env;
        private const string BrandNotFound = "برند درخواستی یافت نشد.";
        private const string BrandDuplicate = "نام برند تکراری است.";
        private readonly IMapper _mapper;
        public BrandController(IUnitOfWork UW, IMapper mapper, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> Getbrands(string search, string order, int offset, int limit, string sort)
        {
            List<BrandViewModel> brands;
            int total = _UW.BaseRepository<Brand>().CountEntities();
            if (!search.HasValue())
                search = "";
            if (limit == 0)
                limit = total;
            if (sort == "نام برند")
            {
                if (order == "asc")
                    brands = await _UW.BrandRepository.GetPaginateBrandsAsync(offset, limit, "Name", search);
                else
                    brands = await _UW.BrandRepository.GetPaginateBrandsAsync(offset, limit, "Name desc", search);
            }
            else if (sort == "تعداد محصولات")
            {
                if (order == "asc")
                    brands = await _UW.BrandRepository.GetPaginateBrandsAsync(offset, limit, "ProductsNumber", search);
                else
                    brands = await _UW.BrandRepository.GetPaginateBrandsAsync(offset, limit, "ProductsNumber desc", search);
            }
            else
                brands = await _UW.BrandRepository.GetPaginateBrandsAsync(offset, limit, "Id desc", search);

            if (search != "")
                total = brands.Count();

            return Json(new { total, rows = brands });
        }
        [HttpGet, AjaxOnly, DisplayName("درج/ویرایش برند")]
        public async Task<IActionResult> RenderBrand(int? BrandId)
        {
            var BrandViewModel = new BrandViewModel();
            if (BrandId != null)
            {
                var Brand = await _UW.BaseRepository<Brand>().FindByIdAsync(BrandId);
                if (Brand != null)
                {
                    BrandViewModel = _mapper.Map<BrandViewModel>(Brand);
                }
                else
                    ModelState.AddModelError(string.Empty, BrandNotFound);
            }
            return PartialView("_RenderBrand", BrandViewModel);
        }
        [HttpPost, AjaxOnly]
        public async Task<IActionResult> CreateOrUpdateBrand(BrandViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ImageFile != null)
                    viewModel.Image = await _UW.BrandRepository.CheckBrandImage(viewModel.ImageFile.FileName);
                if (viewModel.Id != 0)
                {
                    var Brand = await _UW.BaseRepository<Brand>().FindByIdAsync(viewModel.Id);
                    if (Brand != null)
                    {
                        if (viewModel.ImageFile != null)
                        {
                            await viewModel.ImageFile.UploadFileAsync($"{_env.WebRootPath}/Data/Brand/{viewModel.Image}");
                            FileExtensions.DeleteFile($"{_env.WebRootPath}/Data/Brand/{Brand.Image}");
                        }
                        _UW.BaseRepository<Brand>().Update(_mapper.Map(viewModel, Brand));
                        await _UW.Commit();
                        TempData["notification"] = "ویرایش برند با موفقیت انجام شد.";
                    }
                    else
                        ModelState.AddModelError(string.Empty, BrandNotFound);
                }
                else
                {
                    var oldBrand =  _UW.BaseRepository<Brand>().FindByConditionAsync(t => t.Name == viewModel.Name).Result.FirstOrDefault();
                    if(oldBrand!=null)
                        ModelState.AddModelError(string.Empty, BrandDuplicate);
                    else
                    {
                        if (viewModel.ImageFile != null)
                        {
                            await viewModel.ImageFile.UploadFileAsync($"{_env.WebRootPath}/Data/Brand/{viewModel.Image}");
                        }
                        await _UW.BaseRepository<Brand>().CreateAsync(_mapper.Map<Brand>(viewModel));
                        await _UW.Commit();
                        TempData["notification"] = "برند با موفقیت اضافه شد.";
                    }
                }
            }
            return PartialView("_RenderBrand", viewModel);
        }
        [HttpGet, AjaxOnly]
        [DisplayName("حذف")]
        public async Task<IActionResult> Delete(int? brandId)
        {
            if (brandId == null)
                ModelState.AddModelError("", BrandNotFound);
            else
            {
                var Brand = await _UW.BaseRepository<Brand>().FindByIdAsync(brandId);
                if (Brand == null)
                    ModelState.AddModelError("", BrandNotFound);
                else
                    return View("_DeleteConfirmation", Brand);
            }
            return View("_DeleteConfirmation");
        }
        [HttpPost, AjaxOnly, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Brand model)
        {
            var Brand = await _UW.BaseRepository<Brand>().FindByIdAsync(model.Id);
            if (Brand != null)
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/Data/Brand/{Brand.Image}");
                _UW.BaseRepository<Brand>().Delete(Brand);
                await _UW.Commit();
                TempData["notification"] = "حذف برند با موفقیت انجام شد.";
                return PartialView("_DeleteConfirmation", model);
            }
            else
                ModelState.AddModelError("", BrandNotFound);
            return PartialView("_DeleteConfirmation");
        }
        [HttpGet, AjaxOnly]
        public IActionResult DeleteGroup()
        {
            return PartialView("_DeleteGroup");
        }
        [HttpPost, AjaxOnly, ActionName("DeleteGroup")]
        public async Task<IActionResult> DeletedGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError("", "هیچ برندی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var Brand = await _UW.BaseRepository<Brand>().FindByIdAsync(int.Parse(item));
                    FileExtensions.DeleteFile($"{ _env.WebRootPath}/Data/Brand/{ Brand.Image}");
                    _UW.BaseRepository<Brand>().Delete(Brand);
                }
                await _UW.Commit();
                TempData["notification"] = "حذف گروهی با موفقیت انجام شد.";
            }
            return PartialView("_DeleteGroup");
        }
    }
}
