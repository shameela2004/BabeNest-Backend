using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;

namespace BabeNest_Backend.Services
{
    public class CartService :ICartService
    {
        private readonly ICartRepository _repo;
        private readonly IMapper _mapper;

        public CartService(ICartRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartDto>> GetCartAsync(int userId)
        {
            var cart = await _repo.GetUserCartAsync(userId);
            return cart.Select(c => new CartDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ProductImage = c.Product.Image,
                ProductPrice = c.Product.Price,
                Quantity = c.Quantity
            });
        }

        public async Task<CartDto> AddToCartAsync(int userId, CreateCartDto dto)
        {
            var existing = await _repo.GetCartItemAsync(userId, dto.ProductId);
            if (existing != null)
            {
                // Increment quantity by 1 automatically
                existing.Quantity += 1;
                await _repo.UpdateAsync(existing);
                return _mapper.Map<CartDto>(existing);
            }

            var cart = new Cart
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = 1 // always 1 when adding
            };

            await _repo.AddAsync(cart);
            var added = await _repo.GetCartItemAsync(userId, dto.ProductId);

            return _mapper.Map<CartDto>(added);
        }

        public async Task<CartDto?> UpdateCartItemAsync(int userId, int cartId, int quantity)
        {
            var existing = await _repo.GetCartItemByIdAsync(cartId);
            if (existing == null || existing.UserId != userId) return null; // check ownership

            existing.Quantity = quantity;
            await _repo.UpdateAsync(existing);

            return _mapper.Map<CartDto>(existing);
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int cartId)
        {
            var item = await _repo.GetCartItemByIdAsync(cartId);
            if (item == null || item.UserId != userId) return false;

            await _repo.DeleteAsync(item);
            return true;
        }
        public async Task<bool> ClearCartAsync(int userId)
{
    var items = await _repo.GetUserCartAsync(userId);
    if (!items.Any()) return false;

    await _repo.ClearCartAsync(userId);
    return true;
}


    }
}
