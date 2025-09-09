using BabeNest_Backend.DTOs;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BabeNest_Backend.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")] // both can access, but actions are scoped
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Get logged-in user profile
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                //var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var user = await _userService.GetByIdAsync(userId);
                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal error", detail = ex.Message });
            }
        }

        //Update your own profile
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
        {
            //var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            var updatedUser = await _userService.UpdateAsync(userId, dto);
            if (updatedUser == null) return NotFound();

            return Ok(updatedUser);
        }
    }
    }
