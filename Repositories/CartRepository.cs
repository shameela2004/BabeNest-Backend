using BabeNest_Backend.Data;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Repositories
{
    public class CartRepository : ICartRepository
    {
        public readonly BabeNestDbContext _context;  
        public CartRepository(BabeNestDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Cart>> GetUserCartAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
        public async Task<Cart?> GetCartItemAsync(int userId, int productId)
        {
            return await _context.Carts
     .Include(c => c.Product)
     .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        }

        public async Task AddAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            _context.Carts.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

    }
}
