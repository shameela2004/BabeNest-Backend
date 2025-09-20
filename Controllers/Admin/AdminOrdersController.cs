using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
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
            if (orders == null) return NotFound(ApiResponse<Object>.FailResponse("There are no orders"));
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders fetched successfully", 200));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound(ApiResponse<Object>.FailResponse("There is no order with the id"));
            return Ok(ApiResponse<OrderDto>.SuccessResponse(order,"Order fetched successfully"));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int statusId)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if(order.OrderStatusId == 4) //cancelled
                return BadRequest(ApiResponse<Object>.FailResponse("Cannot update status of a cancelled order"));

            var success = await _orderService.UpdateOrderStatusAsync(id, statusId);
            if (success == null) 
                return NotFound(ApiResponse<Object>.FailResponse("Failed to update the order Status"));
            return Ok(ApiResponse<object>.SuccessResponse($"Order status updated to {statusId}"));
        }

        //filtering and sorting
        [HttpGet("filter")]
        public async Task<IActionResult> FilterOrders([FromQuery] int? statusId, [FromQuery] DateTime? startDate,
                                              [FromQuery] DateTime? endDate, [FromQuery] string? searchTerm, [FromQuery] int page=1, [FromQuery] int pageSize=10)
        {
            var orders = await _orderService.FilterOrdersAsync(statusId, startDate, endDate, searchTerm,page,pageSize);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders fetched successfully", 200));
        }


    }
}
