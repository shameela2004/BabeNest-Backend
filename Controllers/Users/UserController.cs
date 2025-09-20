using BabeNest_Backend.DTOs;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BabeNest_Backend.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                //var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var user = await _userService.GetByIdAsync(userId);
                if (user == null) return NotFound(ApiResponse<object>.FailResponse("Login first"));

                return Ok(ApiResponse<UserProfileDto>.SuccessResponse(user,"User profile fetched succcessfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("An unexpected error occured. Please try again later..!"));
            }
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
        {
            //var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            var updatedUser = await _userService.UpdateAsync(userId, dto);
            if (updatedUser == null) return NotFound(ApiResponse<object>.FailResponse("Login first. User is not found"));

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(updatedUser,"Profile updated Successfully."));
        }
    }
    }
