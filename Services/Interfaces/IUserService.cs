using BabeNest_Backend.DTOs;

namespace BabeNest_Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterUserDto dto, string passwordHash);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
        Task<AdminUserProfileDto?> GetUserProfileAsync(int userId);
        Task<bool> BlockUserAsync(int id);
        Task<bool> UnblockUserAsync(int id);
    }
}
