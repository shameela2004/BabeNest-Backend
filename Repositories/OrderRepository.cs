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

        public OrderRepository(BabeNestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id, int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<bool> HasUserReceivedProductAsync(int userId, int productId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .AnyAsync(o => o.UserId == userId
                            && o.Items.Any(i => i.ProductId == productId)
                            && o.Status == "Delivered");
        }

        // -------- Admin methods --------
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User) // so we can map UserName
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdForAdminAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<Order>> FilterOrdersAsync(string? status, DateTime? startDate, DateTime? endDate, string? searchTerm)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(o => o.CustomerEmail.Contains(searchTerm) || o.CustomerName.Contains(searchTerm));


            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }


    }
}
