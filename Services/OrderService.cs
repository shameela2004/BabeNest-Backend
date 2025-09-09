using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Services
{
    public class OrderService :IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo; // optional fallback if cart items don't include Product
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            // 1) Load cart items for user (must include Product navigation ideally)
            var cartItems = (await _cartRepo.GetUserCartAsync(userId)).ToList();
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            // 2) Map customer details from DTO -> Order (AutoMapper copies the basic fields)
            var order = _mapper.Map<Order>(dto);

            // 3) Fill system/computed fields
            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";
            order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6)}";
            order.Items = new List<OrderItem>();

            decimal total = 0m;

            // 4) Build OrderItems from CartItems (snapshot product price)
            foreach (var ci in cartItems)
            {
                // try product from cart navigation first
                var product = ci.Product;
                if (product == null)
                {
                    // fallback: fetch from product repo if needed
                    product = await _productRepo.GetByIdAsync(ci.ProductId)
                              ?? throw new InvalidOperationException($"Product {ci.ProductId} not found.");
                }

                var price = product.Price;
                var orderItem = new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = price
                };

                total += price * ci.Quantity;
                order.Items.Add(orderItem);
            }

            order.TotalAmount = total;

            // 5) Save order
            var createdOrder = await _orderRepo.CreateOrderAsync(order);

            // 6) Clear the cart (delete each cart item)
            foreach (var ci in cartItems)
            {
                await _cartRepo.DeleteAsync(ci);
            }

            // 7) Map saved entity -> OrderDto for response
            var orderDto = _mapper.Map<OrderDto>(createdOrder);
            return orderDto;
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

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string status)
        {
            var updated = await _orderRepo.UpdateOrderStatusAsync(orderId, status);
            return updated == null ? null : _mapper.Map<OrderDto>(updated);
        }
    }
}
