using FaterRasanehMarket.Entities.identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaterRasanehMarket.Entities
{
    public class Like
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public byte LikeNumber { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
