using BabeNest_Backend.Data;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BabeNestDbContext _context;
        public ReviewRepository(BabeNestDbContext context) => _context = context;

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<Review?> GetUserReviewForProductAsync(int userId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task UpdateProductRatingAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                product.Rating = product.Reviews.Any()
                    ? product.Reviews.Average(r => r.Rating)
                    : 0;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }

    }
}
