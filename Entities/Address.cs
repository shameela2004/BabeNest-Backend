namespace BabeNest_Backend.Entities
{
    public class Address
    {
        public int Id { get; set; }   // PK
        public int UserId { get; set; }  // FK to User

        public string FullName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }

        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; }
    }
}

