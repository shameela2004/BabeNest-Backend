using BabeNest_Backend.DTOs;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<ReviewDto?> AddReviewAsync(int userId, string userName, int productId, int rating, string reviewText);
        Task<bool> DeleteReviewAsync(int userId, int reviewId);
    }
}
