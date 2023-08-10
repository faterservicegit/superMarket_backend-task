using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Discount;
using FaterRasanehMarket.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "مدیریت,فروشنده")]
    public class DiscountController : Controller
    {
        private readonly IUnitOfWork _UW;
        private const string DiscountNotFound = "تخفیف درخواستی یافت نشد.";
        private readonly IMapper _mapper;
        public DiscountController(IUnitOfWork UW, IMapper mapper)
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
        [HttpGet]
        public IActionResult Codes()
        {
            return View();
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> GetDiscounts(string search, string order, int offset, int limit, string sort,bool IsCode=false)
        {
            List<DiscountViewModel> Discounts;
            int total = _UW.BaseRepository<Discount>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "درصد")
            {
                if (order == "asc")
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "Percent", search, IsCode);
                else
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "Percent desc", search, IsCode);
            }
            else if (sort == "Id")
            {
                if (order == "asc")
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "Id", search, IsCode);
                else
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "Id desc", search, IsCode);
            }
            else if (sort == "تاریخ شروع")
            {
                if (order == "asc")
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "StartDate", search, IsCode);
                else
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "StartDate desc", search, IsCode);
            }
            else if (sort == "تاریخ انقضا")
            {
                if (order == "asc")
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "EndDate", search, IsCode);
                else
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "EndDate desc", search, IsCode);
            }
            else if (sort == "کالاهای شامل تخفیف")
            {
                if (order == "asc")
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "ProductsCount", search, IsCode);
                else
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "ProductsCount desc", search, IsCode);
            }
            else if (sort == "وضعیت")
            {
                if (order == "asc")
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "IsExpired", search, IsCode);
                else
                    Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "IsExpired desc", search, IsCode);
            }
            else
                Discounts = await _UW.DiscountRepository.GetPaginateDiscountsAsync(offset, limit, "Id desc", search, IsCode);

            if (search != "")
                total = Discounts.Count();

            return Json(new { total, rows = Discounts });
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> RenderDiscount(int? DiscountId, bool IsCode)
        {
            var DiscountViewModel = new DiscountViewModel() { IsPublic = IsCode };
            ViewBag.Products = await _UW.BaseRepository<Product>().FindByConditionAsync(t => t.IsDeleted == false);
            if (DiscountId != null)
            {
                var Discount = _UW.BaseRepository<Discount>().FindByConditionAsync(t => t.Id == DiscountId, null, t => t.ProductsDiscount).Result.FirstOrDefault();
                if (Discount != null)
                {
                    DiscountViewModel = _mapper.Map<DiscountViewModel>(Discount);
                    DiscountViewModel.PersianStartDate = ((DateTime?)DiscountViewModel.StartDate).ConvertMiladiToShamsi("yyyy/MM/dd");
                    if (DiscountViewModel.EndDate != null)
                    {
                        DiscountViewModel.IsInfiniteDiscount = false;
                        DiscountViewModel.PersianEndDate = DiscountViewModel.EndDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                    }
                }
                else
                    ModelState.AddModelError(string.Empty, DiscountNotFound);
            }

            return PartialView("_RenderDiscount", DiscountViewModel);
        }
        [HttpPost, AjaxOnly]
        [DisplayName("درج و ویرایش")]
        public async Task<IActionResult> CreateOrUpdate(DiscountViewModel viewModel)
        {
            ViewBag.Products = await _UW.BaseRepository<Product>().FindByConditionAsync(t => t.IsDeleted == false);
            viewModel.StartDate = viewModel.PersianStartDate.ConvertShamsiToMiladi();
            viewModel.EndDate = viewModel.IsInfiniteDiscount == true ? null : (DateTime?)viewModel.PersianEndDate.ConvertShamsiToMiladi();
            if (viewModel.IsPublic == false)
                ModelState.Remove("Code");
            else if(_UW.DiscountRepository.GetDiscountWithCodeAsync(viewModel.Code).Result!=null)
                ModelState.AddModelError("", "کد تخفیف تکراری است.");
            if (viewModel.ProductIds == null & viewModel.IsPublic == false)
                ModelState.AddModelError("", "انتخاب حداقل یک محصول الزامی است");
            else if (viewModel.EndDate != null & viewModel.EndDate < viewModel.StartDate)
                ModelState.AddModelError("", "تاریخ انتخاب شده نامعتبر است");
            else if (ModelState.IsValid)
            {

                if (viewModel.Id != 0)
                {
                    var discount = _UW.BaseRepository<Discount>().FindByConditionAsync(t => t.Id == viewModel.Id, null, t => t.ProductsDiscount).Result.FirstOrDefault();
                    if (discount != null)
                    {
                        if (viewModel.IsPublic == false)
                            viewModel.ProductsDiscount = viewModel.ProductIds.Select(t => new ProductDiscount { DiscountId = discount.Id, ProductId = int.Parse(t) });
                        _mapper.Map(viewModel, discount);
                        _UW.BaseRepository<Discount>().Update(discount);
                        TempData["notification"] = "ویرایش تخفیف با موفقیت انجام شد.";
                    }
                }
                else
                {
                    var discount = _mapper.Map<Discount>(viewModel);
                    await _UW.BaseRepository<Discount>().CreateAsync(discount);
                    await _UW.Commit();
                    if (viewModel.IsPublic == false)
                        discount.ProductsDiscount = viewModel.ProductIds.Select(t => new ProductDiscount { DiscountId = discount.Id, ProductId = int.Parse(t) }).ToList();
                    TempData["notification"] = "تخفیف با موفقیت اضافه شد.";
                }
                await _UW.Commit();
            }
            return PartialView("_RenderDiscount", viewModel);
        }
        [HttpGet, AjaxOnly]
        [DisplayName("حذف")]
        public async Task<IActionResult> Delete(int? DiscountId)
        {
            if (DiscountId == null)
                ModelState.AddModelError("", DiscountNotFound);
            else
            {
                var Discount = await _UW.BaseRepository<Discount>().FindByIdAsync(DiscountId);
                if (Discount == null)
                    ModelState.AddModelError("", DiscountNotFound);
                else
                    return View("_DeleteConfirmation", Discount);
            }
            return View("_DeleteConfirmation");
        }
        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(Discount model)
        {
            var discount = await _UW.BaseRepository<Discount>().FindByIdAsync(model.Id);
            if (discount != null)
            {
                _UW.BaseRepository<Discount>().Delete(discount);
                await _UW.Commit();
                TempData["notification"] = "حذف تخفیف با موفقیت انجام شد.";
                return PartialView("_DeleteConfirmation", model);
            }
            else
                ModelState.AddModelError("", DiscountNotFound);
            return PartialView("_DeleteConfirmation");
        }
        [HttpPost, AjaxOnly, ActionName("DeleteGroup")]
        [DisplayName("حذف گروهی")]
        public async Task<IActionResult> DeletedGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError("", "هیچ کاربری برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var discount = await _UW.BaseRepository<Discount>().FindByIdAsync(int.Parse(item));
                    _UW.BaseRepository<Discount>().Delete(discount);
                }
                await _UW.Commit();
                TempData["notification"] = "حذف گروهی با موفقیت انجام شد.";
            }
            return PartialView("_DeleteGroup");
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> GetDiscountProducts(int discountId)
        {
            var discount = await _UW.BaseRepository<Discount>().FindByIdAsync(discountId);
            if (discount == null)
                return NotFound();
            if (discount.EndDate > DateTime.Now)
                ViewBag.IsExpired = false;
            else
                ViewBag.IsExpired = true;
            var products = await _UW.DiscountRepository.GetDiscountProductsAsync(discountId);
            if (products == null)
                return NotFound();
            return PartialView("_DiscountProducts", products);
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> GetDiscountOrders(int discountId)
        {
            var orders = await _UW.BaseRepository<Order>().FindByConditionAsync(t=>t.DiscountId==discountId,null,t=>t.Customer);
            ViewBag.DiscountPercent = (await _UW.BaseRepository<Discount>().FindByIdAsync(discountId)).Percent;
            return PartialView("_DiscountOrders", orders);
        }
    }
}
