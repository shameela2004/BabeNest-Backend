using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;

namespace BabeNest_Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;


        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(ProductFilter filters)
        {
            return await _productRepository.GetAllAsync(filters);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<Product?> UpdateAsync(int id, Product updatedProduct)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return null;

            // update fields
            existing.Name = updatedProduct.Name;
            existing.Description = updatedProduct.Description;
            existing.Price = updatedProduct.Price;
            existing.Stock = updatedProduct.Stock;
            existing.Image = updatedProduct.Image;
            existing.CategoryId = updatedProduct.CategoryId;

            await _productRepository.UpdateAsync(existing);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }
    }
}
