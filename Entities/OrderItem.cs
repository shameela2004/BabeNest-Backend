namespace BabeNest_Backend.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }   // PK
        public int OrderId { get; set; }  // FK
        public int ProductId { get; set; } // FK
        public int Quantity { get; set; }
        public decimal Price { get; set; }  // Price at order time

        // Navigation
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
