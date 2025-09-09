namespace BabeNest_Backend.Entities
{
    public class Wishlist
    {
        public int Id { get; set; } // PK
        public int UserId { get; set; } // FK to User
        public int ProductId { get; set; } // FK to Product
        // Navigation properties
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
