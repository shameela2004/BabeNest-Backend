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
// controoler just before crating connection with fron end


//using BabeNest_Backend.DTOs;
//using BabeNest_Backend.Helpers;
//using BabeNest_Backend.Services.Interfaces;
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
//            var userData = new { user.Id, user.Username, user.Email, user.Role };
//            return Ok(ApiResponse<Object>.SuccessResponse(userData,"Registered successfully"));
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
//        {
//            var result = await _authService.LoginAsync(dto.Email, dto.Password);
//            if (result == null) return Unauthorized(ApiResponse<object>.FailResponse("Invalid credentials"));
//            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login Successfull"));
//        }

//        [HttpPost("refresh")]
//        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
//        {
//            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
//            if (result == null) return Unauthorized(ApiResponse<object>.FailResponse("Invalid refresh token"));
//            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result,"Success"));
//        }

//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
//        {
//            var success = await _authService.RevokeTokenAsync(dto.RefreshToken);
//            if (!success) return BadRequest(ApiResponse<object>.FailResponse("Token not found"));
//            return Ok(ApiResponse<Object>.SuccessResponse( "Logged out successfully"));
//        }
//    }
//}







using BabeNest_Backend.DTOs;
using BabeNest_Backend.Helpers;
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

        // ✅ REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            var userData = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userData, "Registered successfully"));
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto.Email, dto.Password);
            if (result == null)
                return Unauthorized(ApiResponse<object>.FailResponse("Invalid credentials"));

            // inside Login and Refresh methods where you append cookie:
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                // For cross-site cookie you need SameSite=None.
                // For dev you can set Secure=false if your frontend is http; in production use Secure=true.
                SameSite = SameSiteMode.None,
                Secure = Request.IsHttps, // true if request came via https; safe fallback
                Expires = result.RefreshTokenExpiry
            };

            Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                AccessToken = result.AccessToken,
                User = result.User
            }, "Login successful"));
        }

        // ✅ REFRESH TOKEN
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(ApiResponse<object>.FailResponse("No refresh token found"));

            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result == null)
                return Unauthorized(ApiResponse<object>.FailResponse("Invalid refresh token"));

            // inside Login and Refresh methods where you append cookie:
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                // For cross-site cookie you need SameSite=None.
                // For dev you can set Secure=false if your frontend is http; in production use Secure=true.
                SameSite = SameSiteMode.None,
                Secure = Request.IsHttps, // true if request came via https; safe fallback
                Expires = result.RefreshTokenExpiry
            };

            Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                AccessToken = result.AccessToken,
                User = result.User
            }, "Token refreshed"));
        }

        // ✅ LOGOUT
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(ApiResponse<object>.FailResponse("No refresh token found"));

            var success = await _authService.RevokeTokenAsync(refreshToken);
            if (!success)
                return BadRequest(ApiResponse<object>.FailResponse("Token not found"));

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                SameSite = SameSiteMode.None,
                Secure = Request.IsHttps
            });

            return Ok(ApiResponse<object>.SuccessResponse("Logged out successfully"));
        }
    }
}

