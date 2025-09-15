using BabeNest_Backend.DTOs;

namespace BabeNest_Backend.Services.Interfaces
{
   
        public interface ICartService
        {
            Task<IEnumerable<CartDto>> GetCartAsync(int userId);
            Task<CartDto> AddToCartAsync(int userId, CreateCartDto dto);
            Task<CartDto?> UpdateCartItemAsync(int userId, int cartId, int quantity);
            Task<bool> RemoveFromCartAsync(int userId, int cartId);
            Task<bool> ClearCartAsync(int userId);


        }
    }

