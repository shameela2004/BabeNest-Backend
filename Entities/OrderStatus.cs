namespace BabeNest_Backend.Entities
{
    public class OrderStatus
    {
        public int Id { get; set; }          // e.g., 1, 2, 3...
        public string Name { get; set; }     // "Pending", "Shipped", etc.
        public ICollection<Order> Orders { get; set; }

    }
}
