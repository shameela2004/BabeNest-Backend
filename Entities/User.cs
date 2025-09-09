namespace BabeNest_Backend.Entities
{
    public class User
    {
        public int Id { get; set; }   // PK
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // Store hashed password
        public string Role { get; set; } = "User"; // Admin / User
        public bool Blocked { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
