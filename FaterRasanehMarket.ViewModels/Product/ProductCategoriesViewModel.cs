using FaterRasanehMarket.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaterRasanehMarket.ViewModels.Product
{
    public class ProductCategoriesViewModel
    {
        public ProductCategoriesViewModel(List<TreeViewCategory> categories, string[] categoryIds)
        {
            Categories = categories;
            CategoryIds = categoryIds;
        }

        public List<TreeViewCategory> Categories { get; set; }
        public string[] CategoryIds { get; set; }
    }
}
