namespace BabeNest_Backend.Entities
{
    public class Review
    {
        public int Id { get; set; } // PK

        // Either store FK to User, OR just username
        public int? UserId { get; set; } // optional, keep real DB link
        public User User { get; set; }

        public string UserName { get; set; } // keep frontend compatibility

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Rating { get; set; }

        // Match JSON naming ("review" instead of "comment")
        public string ReviewText { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

    }
}
