using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Repositories;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using BabeNest_Backend.Services.PaymentService;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BabeNest_Backend.Services
{
    public class OrderService :IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo; // optional fallback if cart items don't include Product
        private readonly IMapper _mapper;
        private readonly RazorpayService _razorpayService;


        // Define constants for IDs to avoid magic numbers
        private const int COD_ID = 1;       // UPDATED: PaymentMethod Id for COD
        private const int ONLINE_ID = 2;    // UPDATED: PaymentMethod Id for Online
        private const int PENDING_PAYMENT_ID = 1; // UPDATED: PaymentStatus Pending
        private const int PAID_PAYMENT_ID = 2;    // UPDATED: PaymentStatus Paid
        private const int FAILED_PAYMENT_ID = 3; // UPDATED: PaymentStatus Failed
        private const int DELIVERED_STATUS_ID = 3; // UPDATED: OrderStatus Delivered
        private const int CANCELLED_STATUS_ID = 4; 

        public OrderService(
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IMapper mapper,
            RazorpayService razorpayService)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
            _razorpayService = razorpayService;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var cartItems = (await _cartRepo.GetUserCartAsync(userId)).ToList();
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var order = _mapper.Map<Order>(dto);

            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.OrderStatusId = 1; // Pending
            order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6]}";
            order.Items = new List<OrderItem>();

            decimal total = 0m;

            foreach (var ci in cartItems)
            {
                var product = ci.Product ?? await _productRepo.GetByIdAsync(ci.ProductId)
                                ?? throw new InvalidOperationException($"Product {ci.ProductId} not found.");

                var orderItem = new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = product.Price,
                    Product = product // ✅ include the product so AutoMapper can map image

                };

                total += product.Price * ci.Quantity;
                order.Items.Add(orderItem);
            }

            order.TotalAmount = total;

            // Payment handling
            if (dto.PaymentMethodId == COD_ID)
            {
                order.PaymentMethodId = COD_ID;
                order.PaymentStatusId = PENDING_PAYMENT_ID; // Marked paid after delivery
            }
            else
            {
                order.PaymentMethodId = ONLINE_ID;
                order.PaymentStatusId = PENDING_PAYMENT_ID; // Will be updated after Razorpay verification

                // 🔹 Call Razorpay API
                var razorOrder = _razorpayService.CreateOrder((int)total);

                // Save Razorpay order id in DB
                order.RazorpayOrderId = razorOrder["id"].ToString();
            }

            var createdOrder = await _orderRepo.CreateOrderAsync(order);

            // Clear user cart
            foreach (var ci in cartItems)
            {
                await _cartRepo.DeleteAsync(ci);
            }

            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId, userId);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, int statusId)
        {
            var updated = await _orderRepo.UpdateOrderStatusAsync(orderId, statusId);
            return updated == null ? null : _mapper.Map<OrderDto>(updated);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetOrderByIdForAdminAsync(id);
            return _mapper.Map<OrderDto?>(order);
        }
        //public async Task<IEnumerable<OrderDto>> FilterOrdersAsync(int? statusId, DateTime? startDate, DateTime? endDate, string? serachTerm,int page,int pageSize)
        //{
        //    var orders = await _orderRepo.FilterOrdersAsync(statusId, startDate, endDate, serachTerm,page,pageSize);
        //    return _mapper.Map<IEnumerable<OrderDto>>(orders);
        //}

        public async Task<PagedResult<OrderDto>> FilterOrdersAsync(
    int? statusId, DateTime? startDate, DateTime? endDate,
    string? searchTerm, int page, int pageSize)
        {
            var result = await _orderRepo.FilterOrdersAsync(statusId, startDate, endDate, searchTerm, page, pageSize);

            return new PagedResult<OrderDto>
            {
                Items = _mapper.Map<IEnumerable<OrderDto>>(result.Items),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }



        //public async Task<OrderDto?> VerifyPaymentAsync(int orderId, string razorpayOrderId, string paymentId, string signature)
        //{
        //    var order = await _orderRepo.GetOrderByIdForAdminAsync(orderId);
        //    if (order == null) return null;

        //    var dto = new VerifyPaymentDto
        //    {
        //        RazorpayOrderId =  razorpayOrderId,
        //        RazorpayPaymentId = paymentId,
        //        RazorpaySignature = signature
        //    };

        //    bool isValid = _razorpayService.VerifyPayment(dto);

        //    if (isValid)
        //        order = await _orderRepo.UpdatePaymentAsync(orderId, PAID_PAYMENT_ID, paymentId, signature);
        //    else
        //        order = await _orderRepo.UpdatePaymentAsync(orderId, FAILED_PAYMENT_ID, paymentId, signature);

        //    return _mapper.Map<OrderDto>(order);
        //}

        public async Task<OrderDto?> VerifyPaymentAsync(VerifyPaymentDto dto)
        {
            var order = await _orderRepo.GetOrderByIdForAdminAsync(dto.OrderId);
            if (order == null) return null;

            // Use frontend values directly for verification
            bool isValid = _razorpayService.VerifyPayment(dto);

            if (isValid)
            {
                order = await _orderRepo.UpdatePaymentAsync(
                    dto.OrderId,
                    PAID_PAYMENT_ID,
                    dto.RazorpayOrderId,
                    dto.RazorpayPaymentId,
                    dto.RazorpaySignature
                );
            }
            else
            {
                order = await _orderRepo.UpdatePaymentAsync(
                    dto.OrderId,
                    FAILED_PAYMENT_ID,
                    dto.RazorpayOrderId,
                    dto.RazorpayPaymentId,
                    dto.RazorpaySignature
                );
            }

            return _mapper.Map<OrderDto>(order);
        }

    }

}

