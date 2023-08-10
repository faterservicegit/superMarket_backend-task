using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaterRasanehMarket.Entities
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public virtual Product Product { get; set; }
    }
}
