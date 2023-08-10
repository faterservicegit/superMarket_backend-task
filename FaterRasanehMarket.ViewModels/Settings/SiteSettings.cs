namespace FaterRasanehMarket.ViewModels.Settings
{
    public class SiteSettings
    {
        public AdminUserSeed AdminUserSeed { get; set; }
        public SiteInfo SiteInfo { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public OrderSettings OrderSettings{ get; set; }
    }
    public class AdminUserSeed
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }

    }
    public class SiteInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
    public class EmailSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
    public class OrderSettings
    {
        public bool IsTakingOrder { get; set; }
        public bool OfflinePayment { get; set; }
        public bool OnlinePayment { get; set; }
        public int ShippingCost { get; set; }
        public int MinOrderForFreeShipping { get; set; }
    }
}
