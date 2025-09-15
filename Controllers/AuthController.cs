//using BabeNest_Backend.DTOs;
//using BabeNest_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace BabeNest_Backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthService _authService;
//        public AuthController(IAuthService authService)
//        {
//            _authService = authService;
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
//        {
//            var user = await _authService.RegisterAsync(dto);
//            return Ok(new { user.Id, user.Username, user.Email, user.Role });
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
//        {
//            var token = await _authService.LoginAsync(dto.Email, dto.Password);
//            if (token == null) return Unauthorized("Invalid credentials");
//            return Ok(new { Token = token });
//        }
//    }
//}


using BabeNest_Backend.DTOs;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto.Email, dto.Password);
            if (result == null) return Unauthorized("Invalid credentials");
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            if (result == null) return Unauthorized("Invalid refresh token");
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var success = await _authService.RevokeTokenAsync(dto.RefreshToken);
            if (!success) return BadRequest("Token not found");
            return Ok(new { Message = "Logged out successfully" });
        }
    }
}
