using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId);
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, int statusId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<PagedResult<OrderDto>> FilterOrdersAsync(
            int? statusId, DateTime? startDate, DateTime? endDate,
            string? searchTerm, int page, int pageSize);
        Task<OrderDto?> VerifyPaymentAsync(VerifyPaymentDto dto);
    }
}

