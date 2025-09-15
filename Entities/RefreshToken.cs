namespace BabeNest_Backend.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }   // the actual refresh token string
        public DateTime ExpiryTime { get; set; }
        public bool IsRevoked { get; set; } = false;

        // Relation to User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
