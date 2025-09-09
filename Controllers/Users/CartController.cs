using BabeNest_Backend.DTOs;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BabeNest_Backend.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await _service.GetCartAsync(userId));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateCartDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var result = await _service.AddToCartAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(int productId, UpdateCartDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var updated = await _service.UpdateCartItemAsync(userId, productId, dto.Quantity);
            return updated != null ? Ok(updated) : NotFound();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var removed = await _service.RemoveFromCartAsync(userId, productId);
            return removed ? Ok(new { message = "Removed" }) : NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var cleared = await _service.ClearCartAsync(userId);
            return cleared ? Ok(new { message = "Cart cleared" }) : NotFound();
        }

    }
}
