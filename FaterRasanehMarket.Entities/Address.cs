using FaterRasanehMarket.Entities.identity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaterRasanehMarket.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string Title { get; set; }
        public string FullAddress { get; set; }
        public bool IsDefualt { get; set; }
        public string PhoneNumber { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
