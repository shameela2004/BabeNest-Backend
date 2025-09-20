using BabeNest_Backend.DTOs;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BabeNest_Backend.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles ="User")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            if(wishlist == null || !wishlist.Any())
                return NotFound(ApiResponse<string>.FailResponse("Wishlist is empty", 404));
            return Ok(ApiResponse<IEnumerable<WishlistDto>>.SuccessResponse(wishlist,"Wishlist retrieved successfully"));
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var item = await _wishlistService.AddToWishlistAsync(userId, productId);
            return Ok(ApiResponse<WishlistDto>.SuccessResponse(item,"Item added to wishlist"));
        }

        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var success = await _wishlistService.RemoveFromWishlistAsync(userId, wishlistId);
            return success ? Ok(ApiResponse<object>.SuccessResponse("Item removed from wishlist")) : NotFound(ApiResponse<object>.FailResponse("Item not found in wishlist"));
        }

        [HttpPost("add-to-cart/{wishlistId}")]
        public async Task<IActionResult> MoveToCart(int wishlistId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var result = await _wishlistService.AddToCartFromWishlistAsync(userId, wishlistId);

            if (result == null)
                return NotFound(ApiResponse<object>.FailResponse("Wishlist item not found."));

            return Ok(ApiResponse<CartDto>.SuccessResponse(result,"Added to cart successfully"));
        }
    }
}
