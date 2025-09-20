using BabeNest_Backend.Data;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Services.PaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BabeNest_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly RazorpayService _razorpayService;
        private readonly BabeNestDbContext _context;

        public PaymentController(RazorpayService razorpayService, BabeNestDbContext context)
        {
            _razorpayService = razorpayService;
            _context = context;
        }

        // STEP 1: Create Razorpay order & save it in DB
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found");

            var razorOrder = _razorpayService.CreateOrder((int)order.TotalAmount);

            // Save Razorpay Order Id
            order.RazorpayOrderId = razorOrder["id"].ToString();
            order.OrderStatusId = 1;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                orderId = order.Id,
                razorpayOrderId = order.RazorpayOrderId,
                amount = razorOrder["amount"],
                currency = razorOrder["currency"]
            });
        }

        // STEP 2: Verify payment after checkout
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.RazorpayOrderId == dto.RazorpayOrderId);
            if (order == null)
                return NotFound("Order not found");

            bool isValid = _razorpayService.VerifyPayment(dto);

            if (isValid)
            {
                order.PaymentStatusId = 2;
                order.RazorpayPaymentId = dto.RazorpayPaymentId;
                order.RazorpaySignature = dto.RazorpaySignature;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Payment verified successfully" });
            }

            order.PaymentStatusId = 3;
            await _context.SaveChangesAsync();

            return BadRequest(new { message = "Payment verification failed" });
        }
    }
}
