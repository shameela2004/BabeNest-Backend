// authservice before building connection with front end

//using AutoMapper;
//using BabeNest_Backend.Data;
//using BabeNest_Backend.DTOs;
//using BabeNest_Backend.Entities;
//using BabeNest_Backend.Helpers;
//using BabeNest_Backend.Repositories;
//using BabeNest_Backend.Repositories.Interfaces;
//using BabeNest_Backend.Services.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace BabeNest_Backend.Services
//{
//    public class AuthService : IAuthService
//    {

//        private readonly IUserRepository _userRepository;
//        private readonly IConfiguration _configuration;
//        private readonly BabeNestDbContext _context;
//        private readonly IMapper _mapper;

//        public AuthService(IUserRepository userRepository, IConfiguration configuration, BabeNestDbContext context, IMapper mapper)
//        {
//            _userRepository = userRepository;
//            _configuration = configuration;
//            _context = context;
//            _mapper = mapper;
//        }

//        public async Task<UserDto> RegisterAsync(RegisterUserDto dto, string role = "User")
//        {
//            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
//            if (existingUser != null) throw new InvalidOperationException("Email already registered");

//            var user = _mapper.Map<User>(dto);
//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
//            user.Role = role;
//            user.Blocked = false;
//            user.CreatedAt = DateTime.UtcNow;

//            await _userRepository.AddAsync(user);

//            return _mapper.Map<UserDto>(user);
//        }

//        public async Task<AuthResponseDto?> LoginAsync(string email, string password)
//        {
//            var user = await _userRepository.GetByEmailAsync(email);
//            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
//                return null;

//            if (user.Blocked)
//                throw new UnauthorizedAccessException("Your account has been blocked by the admin.");

//            //  Use Helper
//            var accessToken = JwtHelper.GenerateJwtToken(user, _configuration);
//            var refreshToken = RefreshTokenHelper.GenerateRefreshToken(user.Id);

//            _context.RefreshTokens.Add(refreshToken);
//            await _context.SaveChangesAsync();

//            return new AuthResponseDto
//            {
//                AccessToken = accessToken,
//                RefreshToken = refreshToken.Token,
//                Username=user.Username,
//                Email=email,

//            };
//        }

//        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
//        {
//            var existingToken = _context.RefreshTokens
//                .FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);

//            if (existingToken == null || existingToken.ExpiryTime <= DateTime.UtcNow)
//                return null;

//            var user = await _userRepository.GetByIdAsync(existingToken.UserId);
//            if (user == null) return null;

//            var accessToken = JwtHelper.GenerateJwtToken(user, _configuration);

//            return new AuthResponseDto
//            {
//                AccessToken = accessToken,
//                RefreshToken = existingToken.Token,
//                Username = user.Username,
//                Email =user.Email,
//            };
//        }

//        public async Task<bool> RevokeTokenAsync(string refreshToken)
//        {
//            var existingToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
//            if (existingToken == null) return false;

//            existingToken.IsRevoked = true;
//            _context.RefreshTokens.Update(existingToken);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}




using AutoMapper;
using BabeNest_Backend.Data;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BabeNest_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly BabeNestDbContext _context;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, BabeNestDbContext context, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto dto, string role = "User")
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null) throw new InvalidOperationException("Email already registered");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = role;
            user.Blocked = false;
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            if (user.Blocked)
                throw new UnauthorizedAccessException("Your account has been blocked by the admin.");

            // ✅ Generate tokens
            var accessToken = JwtHelper.GenerateJwtToken(user, _configuration);
            var refreshToken = RefreshTokenHelper.GenerateRefreshToken(user.Id);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiry = refreshToken.ExpiryTime,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = _context.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (existingToken == null || existingToken.ExpiryTime <= DateTime.UtcNow)
                return null;

            var user = await _userRepository.GetByIdAsync(existingToken.UserId);
            if (user == null) return null;

            // ✅ Rotate refresh token (invalidate old, issue new)
            existingToken.IsRevoked = true;

            var newRefreshToken = RefreshTokenHelper.GenerateRefreshToken(user.Id);
            _context.RefreshTokens.Add(newRefreshToken);

            var accessToken = JwtHelper.GenerateJwtToken(user, _configuration);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiry = newRefreshToken.ExpiryTime,
                User = _mapper.Map<UserDto>(user)

            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var existingToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (existingToken == null) return false;

            existingToken.IsRevoked = true;
            _context.RefreshTokens.Update(existingToken);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
    }
}
