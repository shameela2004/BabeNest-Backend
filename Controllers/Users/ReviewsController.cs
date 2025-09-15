using BabeNest_Backend.DTOs;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BabeNest_Backend.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        // GET: api/reviews/product/5
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _service.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
        }

        // POST: api/reviews
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userName = User.FindFirstValue(ClaimTypes.Name);

            var result = await _service.AddReviewAsync(userId, userName, dto.ProductId, dto.Rating, dto.ReviewText);

            if (result == null)
                return BadRequest(new { message = "You can only review delivered products, and only once." });

            return Ok(result);
        }

        // DELETE: api/reviews/{id}
        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _service.DeleteReviewAsync(userId, id);
            if (!success)
                return BadRequest(new { message = "Review not found or not authorized." });

            return Ok(new { message = "Review deleted successfully." });
        }
    }
}
