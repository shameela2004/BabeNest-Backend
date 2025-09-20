using BabeNest_Backend.Data;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BabeNest_Backend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BabeNestDbContext _context;

        private const int COD_ID = 1;
        private const int PENDING_PAYMENT_ID = 1;
        private const int PAID_PAYMENT_ID = 2;
        private const int DELIVERED_STATUS_ID = 3;

        public OrderRepository(BabeNestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                 .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.PaymentStatus)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id, int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o=>o.OrderStatus)
                .Include(o=>o.PaymentMethod)
                .Include(o=>o.PaymentStatus)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return await _context.Orders
       .Include(o => o.OrderStatus)
       .Include(o => o.PaymentMethod)
       .Include(o => o.PaymentStatus)
       .Include(o => o.Items)
           .ThenInclude(i => i.Product)
       .FirstOrDefaultAsync(o => o.Id == order.Id);
        }

        public async Task<Order?> UpdateOrderStatusAsync(int id, int statusId)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;
            order.OrderStatusId = statusId;
            // Automatically mark COD as Paid when delivered
            if (statusId == DELIVERED_STATUS_ID && order.PaymentMethodId == COD_ID && order.PaymentStatusId == PENDING_PAYMENT_ID)
            {
                order.PaymentStatusId = PAID_PAYMENT_ID;
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order?> UpdatePaymentAsync(int orderId, int paymentStatusId, string RazorpayOrderId = null, string razorpayPaymentId = null, string razorpaySignature = null)
        {
            var order = await _context.Orders
                .Include(o => o.PaymentMethod)
                .Include(o => o.PaymentStatus)
                .Include(o => o.OrderStatus)
                  .FirstOrDefaultAsync(o => o.Id == orderId);
            //.FindAsync(orderId);
            if (order == null) return null;

            order.PaymentStatusId = paymentStatusId;

            if (!string.IsNullOrEmpty(RazorpayOrderId))
                order.RazorpayOrderId = RazorpayOrderId;

            if (!string.IsNullOrEmpty(razorpayPaymentId))
                order.RazorpayPaymentId = razorpayPaymentId;

            if (!string.IsNullOrEmpty(razorpaySignature))
                order.RazorpaySignature = razorpaySignature;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return await _context.Orders
       .Include(o => o.PaymentMethod)
       .Include(o => o.PaymentStatus)
       .Include(o => o.OrderStatus)
       .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<bool> HasUserReceivedProductAsync(int userId, int productId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .AnyAsync(o => o.UserId == userId
                            && o.Items.Any(i => i.ProductId == productId)
                            && o.OrderStatusId == DELIVERED_STATUS_ID);
        }

        // -------- Admin methods --------
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User) // so we can map UserName
                .Include(o=>o.PaymentStatus)
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdForAdminAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .Include(o => o.PaymentMethod)
                .Include(o => o.PaymentStatus)
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<Order>> FilterOrdersAsync(int? status, DateTime? startDate, DateTime? endDate, string? searchTerm, int page,int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.OrderStatusId == status);

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(o => o.CustomerEmail.Contains(searchTerm) || o.CustomerName.Contains(searchTerm));

            // Total count before pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var orders = await query
                 .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return  orders;
        }


    }
}
