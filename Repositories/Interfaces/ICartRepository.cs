using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetUserCartAsync(int userId);
        Task<Cart?> GetCartItemAsync(int userId, int productId);
        Task<Cart?> GetCartItemByIdAsync(int cartId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Cart cart);
        Task ClearCartAsync(int userId);
    }
}
