using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FaterRasanehMarket.Entities
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
