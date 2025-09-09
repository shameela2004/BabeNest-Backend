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
                Quantity = c.Quantity
            });
        }

        public async Task<CartDto> AddToCartAsync(int userId, CreateCartDto dto)
        {
            var existing = await _repo.GetCartItemAsync(userId, dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                await _repo.UpdateAsync(existing);
                return _mapper.Map<CartDto>(existing);
            }

            var cart = new Cart
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            await _repo.AddAsync(cart);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var item = await _repo.GetCartItemAsync(userId, productId);
            if (item == null) return false;

            await _repo.DeleteAsync(item);
            return true;
        }
    }
}
