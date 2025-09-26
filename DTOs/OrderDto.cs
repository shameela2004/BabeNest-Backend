namespace BabeNest_Backend.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderStatusId { get; set; }

        // Customer details
        public int? UserId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        // Payment
        public string OrderStatus { get; set; }          // from OrderStatus.Name
        public string PaymentStatus { get; set; }  // from PaymentStatus.Name
        public string PaymentMethod { get; set; }  // from PaymentMethod.Name
        public string ? RazorpayOrderId { get; set; }   // returned when creating Razorpay order

        // Related items
        public List<OrderItemDto> Items { get; set; }
    }

    public class CreateOrderDto
    {
        //public int? UserId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public int PaymentMethodId { get; set; }  // COD or Online
        //public decimal TotalAmount { get; set; }


        //public List<CreateOrderItemDto> Items { get; set; }
    }

    public class UpdateOrderDto
    {
        public int OrderStatusId { get; set; }

    }
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
