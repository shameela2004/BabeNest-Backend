using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface IwishlistRepository
    {
        Task<Wishlist?> GetWishlistItemAsync(int userId, int productId);
        Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int userId);
        Task AddAsync(Wishlist wishlist);
        Task RemoveAsync(Wishlist wishlist);
    }
}
