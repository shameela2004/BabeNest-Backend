using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Exceptions;
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
            if (existing != null)
                throw new BadRequestException("Product is already in your wishlist.");

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
            if (!items.Any())
               throw new NotFoundException("Your wishlist is empty.");
            return _mapper.Map<IEnumerable<WishlistDto>>(items);
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int wishlistId)
        {
            var existing = await _wishlistRepository.GetWishlistByIdAsync(wishlistId);
            if (existing == null)
                 throw new NotFoundException("Wishlist item not found.");
            if (existing.UserId != userId)
                throw new UnauthorizedException("You cannot delete this wishlist item.");

            await _wishlistRepository.RemoveAsync(existing);
            return true;
        }
        public async Task<CartDto?> AddToCartFromWishlistAsync(int userId, int wishlistId)
        {
            var wishlistItem = await _wishlistRepository.GetWishlistByIdAsync(wishlistId);
            if (wishlistItem == null)
                throw new NotFoundException("Wishlist item not found.");

            if (wishlistItem.UserId != userId)
                throw new UnauthorizedException("You cannot access this wishlist item.");

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






//using AutoMapper;
//using BabeNest_Backend.DTOs;
//using BabeNest_Backend.Entities;
//using BabeNest_Backend.Exceptions;  // <-- import your custom exceptions
//using BabeNest_Backend.Repositories.Interfaces;
//using BabeNest_Backend.Services.Interfaces;

//namespace BabeNest_Backend.Services
//{
//    public class WishlistService : IWishlistService
//    {
//        private readonly IwishlistRepository _wishlistRepository;
//        private readonly ICartRepository _cartRepository;
//        private readonly IMapper _mapper;

//        public WishlistService(
//            IwishlistRepository wishlistRepository,
//            ICartRepository cartRepository,
//            IMapper mapper)
//        {
//            _wishlistRepository = wishlistRepository;
//            _cartRepository = cartRepository;
//            _mapper = mapper;
//        }

//        public async Task<WishlistDto> AddToWishlistAsync(int userId, int productId)
//        {
//            var existing = await _wishlistRepository.GetWishlistItemAsync(userId, productId);
//            if (existing != null)
//                throw new BadRequestException("Product is already in your wishlist."); // instead of silent return

//            var wishlist = new Wishlist
//            {
//                UserId = userId,
//                ProductId = productId
//            };

//            await _wishlistRepository.AddAsync(wishlist);
//            var added = await _wishlistRepository.GetWishlistItemAsync(userId, productId);

//            return _mapper.Map<WishlistDto>(added);
//        }

//        public async Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId)
//        {
//            var items = await _wishlistRepository.GetUserWishlistAsync(userId);

//            if (!items.Any())
//                throw new NotFoundException("Your wishlist is empty.");

//            return _mapper.Map<IEnumerable<WishlistDto>>(items);
//        }

//        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
//        {
//            var existing = await _wishlistRepository.GetWishlistItemAsync(userId, productId);
//            if (existing == null)
//                throw new NotFoundException("Wishlist item not found.");

//            await _wishlistRepository.RemoveAsync(existing);
//            return true;
//        }

//        public async Task<CartDto> AddToCartFromWishlistAsync(int userId, int wishlistId)
//        {
//            var wishlistItem = await _wishlistRepository.GetWishlistByIdAsync(wishlistId);
//            if (wishlistItem == null)
//                throw new NotFoundException("Wishlist item not found.");

//            if (wishlistItem.UserId != userId)
//                throw new UnauthorizedException("You cannot access this wishlist item.");

//            var productId = wishlistItem.ProductId;

//            var cartItem = await _cartRepository.GetCartItemAsync(userId, productId);
//            if (cartItem != null)
//            {
//                cartItem.Quantity += 1;
//                await _cartRepository.UpdateAsync(cartItem);
//            }
//            else
//            {
//                var newCart = new Cart
//                {
//                    UserId = userId,
//                    ProductId = productId,
//                    Quantity = 1
//                };
//                await _cartRepository.AddAsync(newCart);
//                cartItem = newCart;
//            }

//            var refreshed = await _cartRepository.GetCartItemAsync(userId, productId);
//            return _mapper.Map<CartDto>(refreshed);
//        }
//    }
//}

