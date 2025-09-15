using BabeNest_Backend.DTOs;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, status);
            if (success == null) return NotFound();
            return Ok(new { Message = $"Order status updated to {status}" });
        }

        //filtering and sorting
        [HttpGet("filter")]
        public async Task<IActionResult> FilterOrders([FromQuery] string? status, [FromQuery] DateTime? startDate,
                                              [FromQuery] DateTime? endDate, [FromQuery] string? searchTerm)
        {
            var orders = await _orderService.FilterOrdersAsync(status, startDate, endDate, searchTerm);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders fetched successfully", 200));
        }


    }
}
