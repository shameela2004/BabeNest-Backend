namespace BabeNest_Backend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }  // e.g., ORD-xxx
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }   // pending, shipped, delivered, cancelled

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
