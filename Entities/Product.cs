namespace BabeNest_Backend.Entities
{
    public class Product
    {
        public int Id { get; set; }   // PK
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Navigation
        //public ICollection<Cart> Carts { get; set; }
        //public ICollection<Wishlist> Wishlists { get; set; }
        //public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
