using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<Review?> GetUserReviewForProductAsync(int userId, int productId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);     

        Task DeleteAsync(Review review);
        Task<Review?> GetByIdAsync(int id);
        Task UpdateProductRatingAsync(int productId);
    }
}
