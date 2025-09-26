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
    [Authorize(Roles = "User")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound(ApiResponse<string>.FailResponse("You don't have any previous orders", 404));
            return Ok(ApiResponse<Object>.SuccessResponse(orders, "Orders retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
                return NotFound(ApiResponse<object>.FailResponse("Order not found", statusCode: 404));
            return Ok (ApiResponse<Object>.SuccessResponse(order, "Order retrieved successfully."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id in token.");

            try
            {
                var created = await _orderService.CreateOrderAsync(userId, dto);

                // Special handling for Online orders
                if (dto.PaymentMethodId == 2)  //online
                {
                    // Here you can call RazorpayHelper to create order and return payment details
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        created,
                        RazorpayOrder = new
                        {
                            created.RazorpayOrderId,              
                            created.TotalAmount,     // total to be paid
                            created.PaymentStatus,   // "Pending"
                        }
                    }, "Online order created. Proceed with Razorpay payment."));
                }

                // COD order
                return CreatedAtAction(nameof(GetOrder), new { id = created.Id },
                    ApiResponse<object>.SuccessResponse(created, "COD Order created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", detail = ex.Message });
            }
        }
        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<string>.FailResponse("Invalid request", 400));

            try
            {
                var updatedOrder = await _orderService.VerifyPaymentAsync(dto);

                if (updatedOrder == null)
                    return NotFound(ApiResponse<string>.FailResponse("Order not found or invalid verification", 404));

                return Ok(ApiResponse<object>.SuccessResponse(updatedOrder,
                    updatedOrder.PaymentStatus == "Paid" ? "Payment verified successfully." : "Payment verification failed."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Server error: " + ex.Message, 500));
            }
        }
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var order = await _orderService.GetOrderByIdAsync(id, userId);
            if (order == null) 
                return NotFound( ApiResponse<object>.FailResponse("Order not found", statusCode: 404));

            if (order.OrderStatus == "Cancelled")
                return BadRequest(new { message = "Order is already cancelled." });

            //var result = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
            //return result ? Ok(new { message = "Order cancelled successfully." }) : BadRequest(new { message = "Unable to cancel order." });
            // Block cancellation if online payment is already paid
            if (order.PaymentMethod != "COD" && order.PaymentStatus == "Paid")
                return BadRequest("Cannot cancel an order that has already been paid online.");
            if (order.OrderStatus == "Pending")
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, 4);
                if (updatedOrder == null)
                    return BadRequest(new { message = "Unable to cancel order." });

                return Ok(ApiResponse<Object>.SuccessResponse("Order Cancelled Successfully."));
            }
                return BadRequest(ApiResponse<Object>.FailResponse("Order Cannot be cancelled."));


        }
    }
}
