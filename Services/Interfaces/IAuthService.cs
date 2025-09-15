using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
namespace BabeNest_Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterUserDto dto, string role = "User");
        Task<AuthResponseDto?> LoginAsync(string email, string password);
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);

    }
}
