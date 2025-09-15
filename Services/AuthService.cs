using AutoMapper;
using BabeNest_Backend.Data;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Repositories;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            //  Use Helper
            var accessToken = JwtHelper.GenerateJwtToken(user, _configuration);
            var refreshToken = RefreshTokenHelper.GenerateRefreshToken(user.Id);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
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

            var accessToken = JwtHelper.GenerateJwtToken(user, _configuration);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = existingToken.Token
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
    }
}
    
//    public class AuthService : IAuthService
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IConfiguration _configuration;
//        private readonly IMapper _mapper;

//        public AuthService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
//        {
//            _userRepository = userRepository;
//            _configuration = configuration;
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

//        public async Task<string?> LoginAsync(string email, string password)
//        {
//            var user = await _userRepository.GetByEmailAsync(email);
//            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
//                return null;
//            //  check if blocked
//            if (user.Blocked)
//            {
//                throw new UnauthorizedAccessException("Your account has been blocked by the admin.");
//            }

//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new[]
//                {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.Username),
//                new Claim(ClaimTypes.Email, user.Email),
//                new Claim(ClaimTypes.Role, user.Role)
//            }),
//                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
//                Issuer = _configuration["JwtSettings:Issuer"],
//                Audience = _configuration["JwtSettings:Audience"],
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }
//    }

//}
