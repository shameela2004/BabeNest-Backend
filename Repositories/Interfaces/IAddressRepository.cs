using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(int userId);
        Task<Address?> GetByIdAsync(int id, int userId);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(Address address);
        Task<Address?> GetDefaultAddressAsync(int userId);
    }
}
