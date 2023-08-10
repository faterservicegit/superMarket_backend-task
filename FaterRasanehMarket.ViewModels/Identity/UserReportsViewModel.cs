using System;

namespace FaterRasanehMarket.ViewModels.Identity
{
    public class UserReportsViewModel
    {
        public UserDetails UserDetails{get;set;}
    }
    public class UserDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public int VisitCount { get; set; }
        public string RegisterDateTime { get; set; }
        public long BuyAmount { get; set; }
    }
}
