using BusinessObjects.Entities;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderRepository
    {
        private readonly MyNewStoreDbContext _context;

        public OrderRepository(MyNewStoreDbContext context)
        {
            _context = context;
        }

        // Get all orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .ToListAsync();
        }

        // Get orders for a specific user
        public async Task<List<Order>> GetUserOrdersAsync(int accountId)
        {
            return await _context.Orders
                .Where(o => o.AccountId == accountId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .ToListAsync();
        }

        // Get order by ID
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Orchid)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // Create a new order
        public async Task<Order> CreateOrderAsync(int accountId, List<OrderDetail> orderDetails)
        {
            // Calculate total amount
            decimal totalAmount = orderDetails.Sum(od => od.Price * od.Quantity) ?? 0;

            // Create new order
            var order = new Order
            {
                AccountId = accountId,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderStatus = "Pending",
                TotalAmount = totalAmount,
                OrderDetails = orderDetails
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // Update order status
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if user owns the order (for authorization)
        public async Task<bool> UserOwnsOrderAsync(int orderId, int accountId)
        {
            return await _context.Orders
                .AnyAsync(o => o.Id == orderId && o.AccountId == accountId);
        }
    }
}