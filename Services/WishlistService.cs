using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;

namespace BabeNest_Backend.Services
{
    public class WishlistService :IWishlistService
    {
        private readonly IwishlistRepository _wishlistRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public WishlistService(IwishlistRepository wishlistRepository, ICartRepository cartRepository,IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<WishlistDto> AddToWishlistAsync(int userId, int productId)
        {
            var existing = await _wishlistRepository.GetWishlistItemAsync(userId, productId);
            if (existing != null) return _mapper.Map<WishlistDto>(existing);

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepository.AddAsync(wishlist);
            var added = await _wishlistRepository.GetWishlistItemAsync(userId, productId);
            return _mapper.Map<WishlistDto>(added);
        }

        public async Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId)
        {
            var items = await _wishlistRepository.GetUserWishlistAsync(userId);
            return _mapper.Map<IEnumerable<WishlistDto>>(items);
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        {
            var existing = await _wishlistRepository.GetWishlistItemAsync(userId, productId);
            if (existing == null) return false;

            await _wishlistRepository.RemoveAsync(existing);
            return true;
        }
        public async Task<CartDto?> AddToCartFromWishlistAsync(int userId, int wishlistId)
        {
            var wishlistItem = await _wishlistRepository.GetWishlistByIdAsync(wishlistId);
            if (wishlistItem == null || wishlistItem.UserId != userId)
                return null;

            var productId = wishlistItem.ProductId;

            var cartItem = await _cartRepository.GetCartItemAsync(userId, productId);
            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                await _cartRepository.UpdateAsync(cartItem);
            }
            else
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                };
                await _cartRepository.AddAsync(newCart);
                cartItem = newCart;
            }

         

            var refreshed = await _cartRepository.GetCartItemAsync(userId, productId);
            return _mapper.Map<CartDto>(refreshed);
        }

    }
}
