using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FaterRasanehMarket.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Parent")]
        public int? ParentCategoryId { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual Category Parent { get; set; }
        public virtual List<Category> Categories { get; set; }
    }
}
