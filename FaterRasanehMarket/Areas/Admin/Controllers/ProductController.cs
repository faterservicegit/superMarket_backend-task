using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Common.Attributes;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "مدیریت,فروشنده")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UW;
        private readonly IWebHostEnvironment _env;
        private const string ProductNotFound = "کالا درخواستی یافت نشد.";
        private readonly IMapper _mapper;
        public ProductController(IUnitOfWork UW, IMapper mapper, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var nonExistentProducts = await _UW.ProductRepository.NonExistentProductsCountAsync();
            if (nonExistentProducts > 0)
                ViewBag.nonExistentProductsAlert = $"{ nonExistentProducts}  محصول وجود دارد که موجودی آن ها درحال اتمام است و یا ناموجود شده است.";
            return View();
        }
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> Getproducts(string search, string order, string sort, int offset, int limit, int categoryId)
        {
            List<ProductViewModel> products;
            int total = await _UW._Context.Products.CountAsync(t => t.IsDeleted == false);
            if (!search.HasValue())
                search = "";
            if (limit == 0)
                limit = total;
            if (sort == "نام")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Name", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Name desc", search, categoryId);
            }
            else if (sort == "قیمت")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Price", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Price desc", search, categoryId);
            }
            else if (sort == "Id")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Id", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Id desc", search, categoryId);
            }
            else if (sort == "بازدید")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "VisitCount", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "VisitCount desc", search, categoryId);
            }
            else if (sort == "موجودی")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Count", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Count desc", search, categoryId);
            }
            else if (sort == "برند")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Brand.Name", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "Brand.Name desc", search, categoryId);
            }
            else if (sort == "محبوبیت")
            {
                if (order == "asc")
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "LikesPrecent", search, categoryId);
                else
                    products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "LikesPrecent desc", search, categoryId);
            }
            else
                products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, "AddDateTime desc", search, categoryId);
            if (search != "")
                total = products.Count();
            return Json(new { total, rows = products });
        }
        [HttpGet, DisplayName("درج و ویرایش")]
        public async Task<IActionResult> CreateOrUpdate(int? ProductId)
        {
            ViewBag.Brands = await _UW.BaseRepository<Brand>().FindAllAsync();
            ProductViewModel ProductViewModel = new ProductViewModel
            {
                ProductCategoriesViewModel = new ProductCategoriesViewModel(await _UW.CategoryRepository.GetAllCategoriesAsync(), null)
            };
            if (ProductId != null)
            {
                var product = await (from n in _UW._Context.Products.Where(t => t.Id == ProductId & t.IsDeleted == false)
                                     .Include(t => t.Images).Include(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                                     select new ProductViewModel
                                     {
                                         Id = n.Id,
                                         Name = n.Name,
                                         Description = n.Description,
                                         Icon = n.Icon,
                                         Count = n.Count,
                                         BrandId = n.BrandId,
                                         CountLimit = n.CountLimit,
                                         Price = n.Price,
                                         ImagesName = n.Images.Select(t => t.Name).ToList()
                                     }).FirstOrDefaultAsync();
                if (product != null)
                {
                    ProductViewModel = _mapper.Map<ProductViewModel>(product);
                    var discount = await _UW._Context.Discounts.Include(t => t.ProductsDiscount).Where(t => t.ProductsDiscount.Count < 2 && t.ProductsDiscount.Select(t => t.ProductId).Contains((int)product.Id) & (t.EndDate == null || t.EndDate > DateTime.Now)).FirstOrDefaultAsync();
                    if (discount != null)
                    {
                        ProductViewModel.DiscountPrecent = discount.Percent;
                        ProductViewModel.IsInfiniteDiscount = discount.EndDate == null;
                        ProductViewModel.DiscountStartDateTime = ((DateTime?)discount.StartDate).ConvertMiladiToShamsi("yyyy/MM/dd");
                        ProductViewModel.DiscountEndDateTime = discount.EndDate == null ? null : discount.EndDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                        ProductViewModel.DiscountedAmount = ProductViewModel.Price - (ProductViewModel.Price * discount.Percent / 100);
                    }
                    ProductViewModel.ProductCategoriesViewModel = new ProductCategoriesViewModel(await _UW.CategoryRepository.GetAllCategoriesAsync(), await _UW._Context.ProductCategories.Where(t => t.ProductId == product.Id).Select(t => t.CategoryId.ToString()).ToArrayAsync());
                }
            }
            return View(ProductViewModel);
        }
        [HttpPost, DisplayName("درج و ویرایش"), ActionName("CreateOrUpdate")]
        public async Task<IActionResult> CreateOrUpdateConfirm(ProductViewModel viewModel)
        {
            Product product = null;
            ViewBag.Brands = await _UW.BaseRepository<Brand>().FindAllAsync();
            if (ModelState.IsValid)
            {
                if (viewModel.Id != null)
                {
                    product = await _UW.ProductRepository.GetProductWithDetails(viewModel.Id);
                    if (product != null)
                    {
                        if (viewModel.IconFile != null)
                        {
                            var IconName = await _UW.ProductRepository.CheckProductIcon(viewModel.IconFile.FileName);
                            var Result = await viewModel.IconFile.UploadImageAsync($"{_env.WebRootPath}/Data/Product/Icon/{IconName}");
                            if ((bool)Result.IsSuccess)
                            {
                                FileExtensions.DeleteFile($"{_env.WebRootPath}/Data/Product/Icon/{product.Icon}");
                                viewModel.Icon = IconName;
                                await _UW.Commit();
                            }
                            else
                            {
                                ModelState.AddModelError("", Result.Errors.ToString());
                                return View(viewModel);
                            }
                        }
                        if (viewModel.ImageFiles != null)
                        {
                            if (product.Images != null)
                                foreach (var oldim in product.Images)
                                    if (!viewModel.ImagesName.Contains(oldim.Name))
                                    {
                                        FileExtensions.DeleteFile($"{_env.WebRootPath}/Data/Product/Images/{oldim.Name}");
                                        _UW.BaseRepository<Image>().Delete(oldim);
                                    }
                            foreach (var image in viewModel.ImageFiles)
                            {
                                var imageName = await _UW.ProductRepository.CheckProductImage(image.FileName);
                                var Result = await image.UploadImageAsync($"{_env.WebRootPath}/Data/Product/Images/{imageName}");
                                if ((bool)Result.IsSuccess)
                                {
                                    await _UW.BaseRepository<Image>().CreateAsync(new Image { ProductId = product.Id, Name = imageName });
                                    viewModel.ImagesName.Add(imageName);
                                }
                                else
                                    ModelState.AddModelError("", Result.Errors.ToString());
                            }
                        }
                        product.Icon = viewModel.Icon;
                        product.Name = viewModel.Name;
                        product.Count = viewModel.Count;
                        product.CountLimit = viewModel.CountLimit;
                        product.Price = viewModel.Price;
                        product.BrandId = viewModel.BrandId == 0 ? null : viewModel.BrandId;
                        product.Description = viewModel.Description;
                        await _UW.Commit();
                    }
                    else
                        return NotFound();
                }
                else
                {
                    string IconName = "ProdcutDefualtIcon.png";
                    if (viewModel.IconFile != null)
                    {
                        IconName = await _UW.ProductRepository.CheckProductIcon(viewModel.IconFile.FileName);
                        await viewModel.IconFile.UploadImageAsync($"{_env.WebRootPath}/Data/Product/Icon/{IconName}");
                    }
                    product = new Product { Name = viewModel.Name, Count = viewModel.Count, Icon = IconName, Price = viewModel.Price, Description = viewModel.Description };
                    await _UW.BaseRepository<Product>().CreateAsync(product);
                    await _UW.Commit();
                    if (viewModel.ImageFiles != null)
                    {
                        foreach (var image in viewModel.ImageFiles)
                        {
                            var imageName = await _UW.ProductRepository.CheckProductImage(image.FileName);
                            await image.UploadImageAsync($"{_env.WebRootPath}/Data/Product/Images/{imageName}");
                            await _UW.BaseRepository<Image>().CreateAsync(new Image { ProductId = product.Id, Name = imageName });
                        }
                        await _UW.Commit();
                    }
                }
                if ((viewModel.DiscountedAmount == null || viewModel.DiscountedAmount == 0))
                {
                    if (product.ProductDiscounts != null)
                        _UW.BaseRepository<ProductDiscount>().DeleteRange(product.ProductDiscounts.Where(t => t.Discount.IsPublic == false & (t.Discount.EndDate == null || t.Discount.EndDate > DateTime.Now) & t.Discount.ProductsDiscount.Count < 2));
                }
                else
                {
                    var discount = new Discount
                    {
                        Percent = (int)viewModel.DiscountPrecent,
                        IsPublic = false,
                        StartDate = viewModel.DiscountStartDateTime.ConvertShamsiToMiladi(),
                        EndDate = viewModel.IsInfiniteDiscount == true ? null : (DateTime?)viewModel.DiscountEndDateTime.ConvertShamsiToMiladi(),
                    };
                    var oldDiscount = await _UW._Context.Discounts.Include(t => t.ProductsDiscount).Where(t => t.ProductsDiscount.Count < 2 && t.ProductsDiscount.Select(t => t.ProductId).Contains(product.Id) && (t.EndDate == null || t.EndDate > DateTime.Now)).FirstOrDefaultAsync();
                    if (oldDiscount != null)
                    {
                        oldDiscount.Percent = discount.Percent;
                        oldDiscount.IsPublic = discount.IsPublic;
                        oldDiscount.StartDate = discount.StartDate;
                        oldDiscount.EndDate = discount.EndDate;
                        _UW.BaseRepository<Discount>().Update(oldDiscount);
                    }
                    else
                    {
                        await _UW.BaseRepository<Discount>().CreateAsync(discount);
                        await _UW.Commit();
                        await _UW.BaseRepository<ProductDiscount>().CreateAsync(new ProductDiscount { ProductId = product.Id, DiscountId = discount.Id });
                    }
                    await _UW.Commit();
                }
                if (product.ProductCategories != null)
                    _UW.BaseRepository<ProductCategory>().DeleteRange(product.ProductCategories);
                if (viewModel.CategoryIds != null)
                    await _UW.BaseRepository<ProductCategory>().CreateRangeAsync(viewModel.CategoryIds.Select(t => new ProductCategory { CategoryId = int.Parse(t), ProductId = product.Id }).ToList());
                await _UW.Commit();
                return RedirectToAction(nameof(Index));
            }
            viewModel.ProductCategoriesViewModel = new ProductCategoriesViewModel(await _UW.CategoryRepository.GetAllCategoriesAsync(), viewModel.CategoryIds);
            //viewModel.ImagesName =product.Images!=null? product.Images.Select(t => t.Name).ToList():null;
            return View(viewModel);
        }
        [DisplayName("حذف")]
        [HttpGet, AjaxOnly]
        public async Task<IActionResult> Delete(int ProductId)
        {
            var product = await _UW.BaseRepository<Product>().FindByIdAsync(ProductId);
            if (product == null || product.IsDeleted == true)
                ModelState.AddModelError("", ProductNotFound);
            else
                return View("_DeleteConfirmation", product);
            return View("_DeleteConfirmation");
        }
        [HttpPost, AjaxOnly, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Product model)
        {
            var product = (await _UW.BaseRepository<Product>().FindByConditionAsync(t => t.Id == model.Id, null, t => t.Images)).FirstOrDefault();
            if (product != null)
            {
                if (product.Images != null)
                    foreach (var image in product.Images)
                        FileExtensions.DeleteFile($"{_env.WebRootPath}/Data/Products/Images/{image.Name}");
                if (product.Icon != "ProdcutDefualtIcon.png")
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/Files/Product/Icon/{product.Icon}");
                var orders = await _UW.BaseRepository<OrderDetail>().FindByConditionAsync(t => t.ProductId == product.Id);
                if (orders.Count() > 0)
                {
                    product.IsDeleted = true;
                    product.BrandId = null;
                }
                else
                    _UW.BaseRepository<Product>().Delete(product);
                await _UW.Commit();
                TempData["notification"] = "حذف کالا با موفقیت انجام شد.";
                return PartialView("_DeleteConfirmation", model);
            }
            else
                ModelState.AddModelError("", ProductNotFound);
            return PartialView("_DeleteConfirmation");
        }
        [DisplayName("حذف گروهی")]
        [HttpPost, AjaxOnly, ActionName("DeleteGroup")]
        public async Task<IActionResult> DeletedGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError("", "هیچ کالایی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var product = (await _UW.BaseRepository<Product>().FindByConditionAsync(t => t.Id == int.Parse(item), null, t => t.Images)).FirstOrDefault();
                    if (product.Images != null)
                        foreach (var image in product.Images)
                            FileExtensions.DeleteFile($"{_env.WebRootPath}/Data/Products/Images/{image.Name}");
                    if (product.Icon != "ProdcutDefualtIcon.png")
                        FileExtensions.DeleteFile($"{_env.WebRootPath}/Files/Products/Icon/{product.Icon}");
                    var orders = await _UW.BaseRepository<OrderDetail>().FindByConditionAsync(t => t.ProductId == product.Id);
                    if (orders.Count() > 0)
                    {
                        product.IsDeleted = true;
                        product.BrandId = null;
                    }
                    else
                        _UW.BaseRepository<Product>().Delete(product);
                }
                await _UW.Commit();
                TempData["notification"] = "حذف گروهی با موفقیت انجام شد.";
            }
            return PartialView("_DeleteGroup");
        }
        public async Task<IActionResult> GetProductDiscounts(int productId)
        {
            var discounts = await _UW.ProductRepository.GetProductDiscounts(productId, true);
            if (discounts == null)
                return NotFound();
            return PartialView("_ProductDiscounts", discounts);
        }
        [HttpGet]
        public IActionResult AddProductsWithExcel()
        {
            return PartialView("_AddProductsWithExcel");
        }
        [HttpPost]
        public async Task<IActionResult> AddProductsWithExcel(IFormFile excelFile)
        {
            if (Path.GetExtension(excelFile.FileName).ToLower() == ".xlsx")
            {
                XSSFWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
                ISheet sheet = workbook.GetSheetAt(0);
                var products = new List<Product>();
                for (int rowIdx = 2; rowIdx <= sheet.LastRowNum; rowIdx++)
                {
                    IRow currentRow = sheet.GetRow(rowIdx);
                    var idcolm = currentRow.GetCell(2).ToString();
                    if (currentRow == null || currentRow.Cells == null || currentRow.Cells.Count < 5 || idcolm == "") break;
                    if (currentRow.GetCell(1).ToString() == "-")
                    {
                        products.Add(new Product
                        {
                            Name = currentRow.GetCell(2).ToString(),
                            Price = int.Parse(currentRow.GetCell(3).ToString()),
                            Count = int.Parse(currentRow.GetCell(4).ToString()),
                            Icon = "ProdcutDefualtIcon.png",
                        });
                    }
                    else
                    {
                        var product = await _UW.BaseRepository<Product>().FindByIdAsync(int.Parse(currentRow.GetCell(1).ToString()));
                        if (product != null)
                        {
                            product.Name = currentRow.GetCell(2).ToString();
                            product.Price = int.Parse(currentRow.GetCell(3).ToString());
                            product.Count = int.Parse(currentRow.GetCell(4).ToString());
                            await _UW.Commit();
                        }
                    }
                }
                if (products.Count > 0)
                {
                    await _UW.BaseRepository<Product>().CreateRangeAsync(products);
                    await _UW.Commit();
                }
            }
            else
            {
                ViewData["Alert"] = "فایل ارسال شده معتبر نمی باشد.";
            }
            return PartialView("_AddProductsWithExcel");
        }
        [HttpGet]
        public async Task<List<Category>> GetCategories()
        {
            return (await _UW.BaseRepository<Category>().FindAllAsync()).ToList();
        }
    }
}
