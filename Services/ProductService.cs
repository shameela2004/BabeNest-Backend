using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;

namespace BabeNest_Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly CloudinaryHelper _cloudinary;



        public ProductService(IProductRepository productRepository, CloudinaryHelper cloudinary)
        {
            _productRepository = productRepository;
            _cloudinary = cloudinary;
        }

        public async Task<(IEnumerable<Product>,int)> GetAllAsync(ProductFilter filters)
        {
            var (products,totalcount)= await _productRepository.GetAllAsync(filters);
            return (products,totalcount);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateAsync(Product product, IFormFile? imageFile)
        {
            if (imageFile != null)
            {
                var (imageUrl, publicId) = await _cloudinary.UploadImageAsync(imageFile);
                product.Image = imageUrl;
                product.ImagePublicId = publicId;
            }

            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<Product?> UpdateAsync(int id, Product updatedProduct, IFormFile? imageFile)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return null;

            // Replace image if new one uploaded
            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(existing.ImagePublicId))
                {
                    await _cloudinary.DeleteImageAsync(existing.ImagePublicId); // delete old
                }

                var (imageUrl, publicId) = await _cloudinary.UploadImageAsync(imageFile);
                existing.Image = imageUrl;
                existing.ImagePublicId = publicId;
            }

            existing.Name = updatedProduct.Name;
            existing.Description = updatedProduct.Description;
            existing.Price = updatedProduct.Price;
            existing.Stock = updatedProduct.Stock;
            existing.CategoryId = updatedProduct.CategoryId;

            await _productRepository.UpdateAsync(existing);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return false;

            if (!string.IsNullOrEmpty(existing.ImagePublicId))
            {
                await _cloudinary.DeleteImageAsync(existing.ImagePublicId);
            }

            return await _productRepository.DeleteAsync(id);
        }
    }
}
