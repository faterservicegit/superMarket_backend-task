using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UW;
        private readonly IHttpContextAccessor _accessor;
        public ProductController(IUnitOfWork UW, IHttpContextAccessor accessor)
        {
            _UW = UW;
            _UW.CheckArgumentIsNull(nameof(_UW));
            _accessor = accessor;
            _accessor.CheckArgumentIsNull(nameof(_accessor));

        }
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var product = await _UW.ProductRepository.GetProductWithDetails(productId);
            if (product == null)
                return NotFound();
            string ipAddress = _accessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            if (User.Identity.IsAuthenticated)
            {
                var visit = (await _UW.BaseRepository<Visit>().FindByConditionAsync(t => t.ProductId == product.Id & t.UserId == int.Parse(User.Identity.GetUserId()))).FirstOrDefault();
                if (visit == null)
                    await _UW.BaseRepository<Visit>().CreateAsync(new Visit { UserId = int.Parse(User.Identity.GetUserId()), ProductId = product.Id, IpAddress = ipAddress, NumberOfVisit = 1 });
                else
                {
                    visit.NumberOfVisit++;
                    visit.IpAddress = ipAddress;
                    visit.LastVisitDateTime = DateTime.Now;
                }
            }
            else
            {
                var visit = (await _UW.BaseRepository<Visit>().FindByConditionAsync(t => t.ProductId == product.Id & t.IpAddress == ipAddress)).FirstOrDefault();
                if (visit == null)
                    await _UW.BaseRepository<Visit>().CreateAsync(new Visit { ProductId = product.Id, IpAddress = ipAddress, NumberOfVisit = 1 });
                else
                {
                    visit.LastVisitDateTime = DateTime.Now;
                    visit.NumberOfVisit++;
                }
            }
            await _UW.Commit();
            return View(product);
        }
        public async Task<IActionResult> CategoryProducts(int categoryId)
        {
            var category = (await _UW.BaseRepository<Category>().FindByConditionAsync(c => c.Id == categoryId, null, t => t.ProductCategories)).FirstOrDefault();
            if (category == null)
                return NotFound();
            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = category.Id,
                ProductsCount = category.ProductCategories.Count,
                Products = await _UW.ProductRepository.GetPaginateProductsAsync(0, 20, "Id Desc", "", categoryId)
            };
            var Categories = await _UW.CategoryRepository.GetCategoryTreeAsync(categoryId);
            ViewData["Categories"] = Categories;

            foreach (var Category in Categories)
                viewModel.CategoryName = Categories.IndexOf(Category) == Categories.Count - 1 ? viewModel.CategoryName + Category.Name : viewModel.CategoryName + Category.Name + "/";
            return View(viewModel);
        }
        public async Task<IActionResult> GetCategoryProductsPagination(int offset, int limit, string orderBy, int categoryId)
        {
            var products = await _UW.ProductRepository.GetPaginateProductsAsync(offset, limit, orderBy, "", categoryId);
            return PartialView("_CategoryProductsPagination", products);
        }
        public async Task<IActionResult> RegisterProducScores(int productId, byte score)
        {
            var product= await _UW.BaseRepository<Product>().FindByIdAsync(productId);
            if (product == null)
                return BadRequest("محصول یافت نشد");
            var userId =int.Parse( User.Identity.GetUserId());
            var like = (await _UW.BaseRepository<Like>().FindByConditionAsync(t => t.ProductId == productId & t.UserId == userId)).FirstOrDefault();
            if(like!=null)
                return BadRequest("شما قبلا امتیاز این محصول را ثبت کرده اید");
            await _UW.BaseRepository<Like>().CreateAsync(new Like {ProductId=productId,UserId=userId,LikeNumber=score });
            await _UW.Commit();
            return Ok($"امتیاز شما در مورد محصول {product.Name} باموفقیت ثبت گردید");
        }
    }
}
