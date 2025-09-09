using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BabeNest_Backend.Services
{
    //public class AuthService :IAuthService
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly IConfiguration _configuration;

    //    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    //    {
    //        _userRepository = userRepository;
    //        _configuration = configuration;
    //    }

    //    public async Task<User> RegisterAsync(string username, string email, string password, string role = "User")
    //    {
    //        var existingUser = await _userRepository.GetByEmailAsync(email);
    //        if (existingUser != null) throw new Exception("Email already exists");

    //        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

    //        var user = new User
    //        {
    //            Username = username,
    //            Email = email,
    //            PasswordHash = hashedPassword,
    //            Role = role
    //        };

    //        await _userRepository.AddAsync(user);
    //        return user;
    //    }

    //    public async Task<string?> LoginAsync(string email, string password)
    //    {
    //        var user = await _userRepository.GetByEmailAsync(email);
    //        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
    //            return null;

    //        // Generate JWT
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(new[]
    //            {
    //                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //                new Claim(ClaimTypes.Name, user.Username),
    //                new Claim(ClaimTypes.Email, user.Email),
    //                new Claim(ClaimTypes.Role, user.Role)
    //            }),
    //            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
    //            Issuer = _configuration["JwtSettings:Issuer"],
    //            Audience = _configuration["JwtSettings:Audience"],
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        return tokenHandler.WriteToken(token);
    //    }
    //}
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
