using FaterRasanehMarket.Data.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Services.Hubs
{
    [Authorize(Roles = "مدیریت,فروشنده")]
    public class SellerHub : Hub
    {
        private readonly IUnitOfWork _UW;
        public SellerHub(IUnitOfWork UW)
        {
            _UW = UW;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
