using BabeNest_Backend.Data;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Repositories
{
    public class ProductRepository :IProductRepository
    {
        private readonly BabeNestDbContext _context;
        public ProductRepository(BabeNestDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync(ProductFilter filters)
        {
            var query = _context.Products
      .Include(p => p.Category)   //include category for filtering
      .Include(p => p.Reviews)    // include reviews to calculate rating
      .AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(filters.SearchTerm) ||
                    p.Description.Contains(filters.SearchTerm));
            }

            if (filters.CategoryId.HasValue)   // 🔹 now use CategoryId
            {
                query = query.Where(p => p.CategoryId == filters.CategoryId.Value);
            }

            if (filters.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);

            if (filters.Rating.HasValue)
                query = query.Where(p => p.Rating >= filters.Rating.Value);


            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetPriceAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception($"Product with Id {productId} not found.");

            return product.Price;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
