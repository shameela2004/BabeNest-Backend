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
    [Authorize(Roles = "User")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null) return NotFound();
            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id in token.");

            try
            {
                var created = await _orderService.CreateOrderAsync(userId, dto);
                return CreatedAtAction(nameof(GetOrder), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // log exception
                return StatusCode(500, new { message = "Server error", detail = ex.Message });
            }
        }

        // PUT: api/order/cancel/5
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            // Only allow cancelling
            var order = await _orderService.GetOrderByIdAsync(id, userId);
            if (order == null) return NotFound();

            if (order.Status == "Cancelled")
                return BadRequest(new { message = "Order is already cancelled." });

            //var result = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
            //return result ? Ok(new { message = "Order cancelled successfully." }) : BadRequest(new { message = "Unable to cancel order." });

            var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
            if (updatedOrder == null)
                return BadRequest(new { message = "Unable to cancel order." });

            return Ok(new { message = "Order cancelled successfully." });

        }
    }
}
