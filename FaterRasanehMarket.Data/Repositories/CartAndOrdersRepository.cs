using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.ViewModels.Order;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Repositories
{
    public class CartAndOrdersRepository : ICartAndOrdersRepository
    {
        private readonly FaterRasanehMarketDBContext _Context;
        public CartAndOrdersRepository(FaterRasanehMarketDBContext Context)
        {
            _Context = Context;
            _Context.CheckArgumentIsNull(nameof(_Context));
        }
        public async Task<List<OrderViewModel>> GetPaginateOrdersAsync(int offset, int limit, string orderBy, string searchText, OrderStatusCode orderStatus,int? userId)
        {
            var OrdersViewModel = await _Context.Orders.Where(n =>n.OrderStatus!=OrderStatusCode.AwaitingPayment&(userId==null || n.CustomerId==userId)&(orderStatus==0 || n.OrderStatus==orderStatus )&& (n.Id.Contains(searchText) || n.Description.Contains(searchText) || n.DispatchNumber.Contains(searchText))).Include(t => t.Discount).Include(t => t.Customer).Include(t => t.Address)
                            .Select(t => new OrderViewModel
                            {
                                Id = t.Id,
                                AmountPaid = t.AmountPaid,
                                Description = t.Description,
                                DispatchNumber = t.DispatchNumber,
                                PaidDateTime = t.PaidDateTime,
                                ShipDateTime = t.ShipDateTime,
                                ShippingCost = t.ShippingCost,
                                PaymentMethod = t.PaymentMethod,
                                DiscountAmount=((t.AmountPaid-t.ShippingCost)*t.Discount.Percent)/(100-t.Discount.Percent),
                                OrderStatus = t.OrderStatus,
                                Customer = $"{t.Customer.FirstName} {t.Customer.LastName}({t.Customer.PhoneNumber})",
                                DiscountCode = t.Discount.Code,
                                }).AsNoTracking().OrderBy(orderBy).Skip(offset).Take(limit).ToListAsync();
            int row = 0;
            foreach (var item in OrdersViewModel)
            {
                item.Row = ++row;
                item.PersianPaidDateTime = item.PaidDateTime.ConvertMiladiToShamsi("HH:mm yyyy/MM/dd");
                item.PersianShipDateTime =item.ShipDateTime==null?"زمان ارسال تعیین نشده است": item.ShipDateTime.ConvertMiladiToShamsi("HH:mm yyyy/MM/dd");
                item.PaymentMethodName =item.PaymentMethod!=0? item.PaymentMethod.GetDisplayName():"";
            }
            return OrdersViewModel;
        }
        public async Task<Order> GetUserCartAsync(int userId)
        {
            var order = await _Context.Orders.Where(t => t.CustomerId == userId & t.OrderStatus == OrderStatusCode.AwaitingPayment)
                .Include(t => t.OrderProducts).ThenInclude(t => t.Product).ThenInclude(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                .Include(t => t.Discount).Include(t=>t.Address).Include(t=>t.Customer).FirstOrDefaultAsync();
            return order;
        }
        public async Task<Order> GetOrderWithDetailsAsync(string orderId)
        {
            var order = await _Context.Orders.Where(t => t.Id == orderId).Include(t => t.OrderProducts).ThenInclude(t => t.Product).ThenInclude(t => t.ProductDiscounts).ThenInclude(t => t.Discount)
                .Include(t => t.Discount).Include(t=>t.Address)
                .Include(t=>t.OrderProducts).ThenInclude(t=>t.Product).ThenInclude(t=>t.Likes)
                .FirstOrDefaultAsync();
            return order;
        }
        public async Task<bool> CheckForCartUpdate(Order order)
        {
            var cartUpdated = false;
            foreach (var orderDetail in order.OrderProducts)
            {
                if (orderDetail.Product.Count == 0 || orderDetail.Product.IsDeleted == true)
                {
                    _Context.OrderDetails.Remove(orderDetail);
                    cartUpdated = true;
                }
                else if (orderDetail.Count > orderDetail.Product.Count)
                {
                    orderDetail.Count = orderDetail.Product.Count;
                    cartUpdated = true;
                }
                var currentPrice = orderDetail.Product.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Count() > 0 ? (orderDetail.Product.Price - (orderDetail.Product.Price * (orderDetail.Product.ProductDiscounts.Select(t => t.Discount).Where(t => t.IsPublic == false & t.StartDate < DateTime.Now & (t.EndDate > DateTime.Now || t.EndDate == null)).Sum(t => t.Percent))) / 100) : orderDetail.Product.Price;
                if (currentPrice != orderDetail.Price)
                {
                    orderDetail.Price = currentPrice;
                    cartUpdated = true;
                }
            }
            if (order.Discount != null & (order.Discount?.StartDate > DateTime.Now || order.Discount?.EndDate < DateTime.Now))
            {
                order.DiscountId = null;
                cartUpdated = true;
            }
            if (cartUpdated == true)
                await _Context.SaveChangesAsync();
            return cartUpdated;

        }
        public async Task<List<Address>> GetUserAddressesAsync(int userId)
        {
            return await _Context.Addresses.Where(t => t.UserId == userId).ToListAsync();
        }
        public async Task<string> GenerateOrderIdAsync()
        {
            var radnom = new Random();
            var id = (radnom.Next(10000000,99999999)).ToString();
            var order = await _Context.Orders.Where(t => t.Id == id).AsNoTracking().FirstOrDefaultAsync();
            while (order != null)
            {
                id = Guid.NewGuid().ToString().Substring(0, 8);
                order = await _Context.Orders.Where(t => t.Id == id).AsNoTracking().FirstOrDefaultAsync();
            }
            return id;
        }
        public async Task<int> CheckNewOrderCountAsync(int IntervalHour = 12)
        {
            return await _Context.Orders.Where(t => t.OrderStatus==OrderStatusCode.Waitingforapproval& t.PaidDateTime > DateTime.Now.AddHours(-1 * IntervalHour)).AsNoTracking().CountAsync();
        }
    }
}
