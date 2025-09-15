using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int id, int userId);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderStatusAsync(int id, string status);
        Task<bool> HasUserReceivedProductAsync(int userId, int productId);

        //  Admin specific
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdForAdminAsync(int id);
        Task<IEnumerable<Order>> FilterOrdersAsync(string? status, DateTime? startDate, DateTime? endDate, string? searchTerm);

    }
}
