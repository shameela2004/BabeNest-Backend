using BabeNest_Backend.DTOs;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync(int userId);
        Task<AddressDto> GetByIdAsync(int userId, int addressId);
        Task<AddressDto> CreateAsync(int userId, CreateAddressDto dto);
        Task<AddressDto> UpdateAsync(int userId, int addressId, UpdateAddressDto dto);
        Task<bool> DeleteAsync(int userId, int addressId);
        Task<bool> SetDefaultAsync(int userId, int addressId);
    }
}
