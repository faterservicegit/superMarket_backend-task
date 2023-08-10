using FaterRasanehMarket.Common;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface ICartAndOrdersRepository
    {
        Task<string> GenerateOrderIdAsync();
        Task<Order> GetOrderWithDetailsAsync(string orderId);
        Task<Order> GetUserCartAsync(int userId);
        Task<bool> CheckForCartUpdate(Order order);
        Task<List<Address>> GetUserAddressesAsync(int userId);
        Task<List<OrderViewModel>> GetPaginateOrdersAsync(int offset, int limit, string orderBy, string searchText, OrderStatusCode orderStatus, int? userId);
        Task<int> CheckNewOrderCountAsync(int IntervalHour = 4);
    }
}
