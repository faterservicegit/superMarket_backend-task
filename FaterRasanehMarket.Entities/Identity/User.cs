using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace FaterRasanehMarket.Entities.identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime? RegisterDateTime { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
