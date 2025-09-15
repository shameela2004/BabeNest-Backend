using BabeNest_Backend.DTOs;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId);
        Task<WishlistDto> AddToWishlistAsync(int userId, int productId);
        Task<bool> RemoveFromWishlistAsync(int userId, int wishlistId);
        Task<CartDto?> AddToCartFromWishlistAsync(int userId, int wishlistId);
    }
}
