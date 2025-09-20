using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product>, int)> GetAllAsync(ProductFilter filters);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task <bool>DeleteAsync(int id);
        Task<decimal> GetPriceAsync(int productId);

    }
}
