using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<(IEnumerable<Product>,int)> GetAllAsync(ProductFilter filters);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product, IFormFile? imageFile);
        Task<Product?> UpdateAsync(int id, Product updatedProduct, IFormFile? imageFile);
        Task<bool> DeleteAsync(int id);
    }
}
