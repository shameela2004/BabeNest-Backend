namespace BabeNest_Backend.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        // Customer details
        public int? UserId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        // Related items
        public List<OrderItemDto> Items { get; set; }
    }

    public class CreateOrderDto
    {
        public int? UserId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        public List<CreateOrderItemDto> Items { get; set; }
    }

    public class UpdateOrderDto
    {
        public string Status { get; set; }
    }

}
