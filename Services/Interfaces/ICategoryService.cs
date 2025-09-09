using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<Category?> UpdateAsync(int id, Category updatedCategory);
        Task<bool> DeleteAsync(int id);
    }
}
