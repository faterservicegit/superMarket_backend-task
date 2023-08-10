using Microsoft.AspNetCore.Identity;


namespace FaterRasanehMarket.Entities.identity
{
    public class RoleClaim : IdentityRoleClaim<int>
    {
        public virtual Role Role { get; set; }
    }
}
