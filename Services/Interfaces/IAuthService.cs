using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
namespace BabeNest_Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto>RegisterAsync(RegisterUserDto dto, string role = "User");
        Task<string?>LoginAsync(string email, string password);
    }
}
