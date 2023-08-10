using System.Collections.Generic;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Product;

namespace FaterRasanehMarket.ViewModels.Category
{
    public class CategoryProductsViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductsCount { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
