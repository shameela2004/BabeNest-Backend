namespace BabeNest_Backend.DTOs
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class CreateWishlistDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }

}
