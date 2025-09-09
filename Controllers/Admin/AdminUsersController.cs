using BabeNest_Backend.DTOs;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        //{
        //    var updatedUser = await _userService.UpdateAsync(id, dto);
        //    if (updatedUser == null) return NotFound();

        //    return Ok(updatedUser);
        //}

        [HttpPut("{id}/block")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var success = await _userService.BlockUserAsync(id);
            if (!success) return NotFound();

            return Ok(new { Message = "User blocked successfully" });
        }

        [HttpPut("{id}/unblock")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var success = await _userService.UnblockUserAsync(id);
            if (!success) return NotFound();

            return Ok(new { Message = "User unblocked successfully" });
        }
    }
}
