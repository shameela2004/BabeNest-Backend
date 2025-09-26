using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int id, int userId);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderStatusAsync(int id, int statusId);
        Task<Order?> UpdatePaymentAsync(int orderId, int paymentStatusId, string RazorpayOrderId = null, string razorpayPaymentId = null, string razorpaySignature = null);

        Task<bool> HasUserReceivedProductAsync(int userId, int productId);

        //  Admin specific
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdForAdminAsync(int id);
        Task<PagedResult<Order>> FilterOrdersAsync(
             int? statusId,
             DateTime? startDate,
             DateTime? endDate,
             string? searchTerm,
             int page,
             int pageSize);
    }
}
