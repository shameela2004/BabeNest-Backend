using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IOrderRepository _orderRepo; // to check delivery
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository repo, IOrderRepository orderRepo, IMapper mapper)
        {
            _repo = repo;
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _repo.GetReviewsByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> AddReviewAsync(int userId, string userName, int productId, int rating, string reviewText)
        {
            // 1. Check if user bought and received product
            var delivered = await _orderRepo.HasUserReceivedProductAsync(userId, productId);
            if (!delivered) return null;

            // 2. Check if review already exists
            var existing = await _repo.GetUserReviewForProductAsync(userId, productId);

            if (existing != null)
            {
                // Override existing review
                existing.Rating = rating;
                existing.ReviewText = reviewText;
                existing.Date = DateTime.Now;

                await _repo.UpdateAsync(existing);
            }
            else
            {
                // Add new review
                var review = new Review
                {
                    UserId = userId,
                    UserName = userName,
                    ProductId = productId,
                    Rating = rating,
                    ReviewText = reviewText,
                    Date = DateTime.Now
                };

                await _repo.AddAsync(review);
                existing = review;
            }

            // Recalculate & update product rating
            await _repo.UpdateProductRatingAsync(productId);

            return _mapper.Map<ReviewDto>(existing);
        }

        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _repo.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId) return false;

            await _repo.DeleteAsync(review);
            // Update product rating
            await _repo.UpdateProductRatingAsync(review.ProductId);
            return true;
        }
    }
}
