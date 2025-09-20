namespace BabeNest_Backend.Entities
{
    public class PaymentStatus
    {
        public int Id { get; set; }          // e.g., 1, 2
        public string Name { get; set; }     // "Pending", "Paid"
        public ICollection<Order> Orders { get; set; }

    }
}
