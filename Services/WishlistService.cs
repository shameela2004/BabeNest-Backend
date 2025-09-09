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
        private readonly IMapper _mapper;

        public WishlistService(IwishlistRepository wishlistRepository, IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
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
            return _mapper.Map<WishlistDto>(wishlist);
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
    }
}
