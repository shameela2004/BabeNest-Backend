namespace BabeNest_Backend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }  // e.g., ORD-xxx
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public int PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public int PaymentMethodId { get; set; }  
        public PaymentMethod PaymentMethod { get; set; }
        



        // Razorpay fields
        public string? RazorpayOrderId { get; set; }   // returned when creating Razorpay order
        public string? RazorpayPaymentId { get; set; } // filled after payment success
        public string? RazorpaySignature { get; set; } // used for verifying payment

        // Customer info
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        // Relationships
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}
