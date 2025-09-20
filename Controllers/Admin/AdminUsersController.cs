using BabeNest_Backend.DTOs;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // only Admin can use this
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var users = await _userService.GetAllAsync();
        //    return Ok(users);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto query)
        {
            var (users, totalCount) = await _userService.GetUsersAsync(query);
            if (totalCount <0) return NotFound(ApiResponse<object>.FailResponse("No users found", statusCode: 404));
            var paginatedData = new
            {
                Data = users,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
            return Ok(ApiResponse<object>.SuccessResponse(paginatedData, "Users retrieved successfully"));
        }


        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var user = await _userService.GetByIdAsync(id);
        //    if (user == null) return NotFound(ApiResponse<object>.FailResponse("No user found"));

        //    return Ok(ApiResponse<UserDto>.SuccessResponse(user,"User retrieved successfully."));
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        //{
        //    var updatedUser = await _userService.UpdateAsync(id, dto);
        //    if (updatedUser == null) return NotFound();

        //    return Ok(updatedUser);
        //}


        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var profile = await _userService.GetUserProfileAsync(id);
            if (profile == null)
                return NotFound(ApiResponse<object>.FailResponse("User not found"));

            return Ok(ApiResponse<AdminUserProfileDto>.SuccessResponse(profile,"User retrieved successfully."));
        }

        [HttpPut("{id}/block")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var success = await _userService.BlockUserAsync(id);
            if (!success) return NotFound(ApiResponse<object>.FailResponse("Failed to block user. User not found"));

            return Ok(ApiResponse<object>.SuccessResponse("User blocked successfully"));
        }

        [HttpPut("{id}/unblock")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var success = await _userService.UnblockUserAsync(id);
            if (!success) return NotFound(ApiResponse<object>.FailResponse("Failed to Unblock user. User not found"));


            return Ok(ApiResponse<object>.SuccessResponse("User Unblocked successfully"));
        }
    }
}
