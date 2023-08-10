using FaterRasanehMarket.Entities.identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FaterRasanehMarket.Entities
{
   public class Visit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public string IpAddress { get; set; }
        public int NumberOfVisit { get; set; }
        public DateTime? LastVisitDateTime { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
