namespace BabeNest_Backend.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime Date { get; set; }

        // Extra fields for better UI
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
    }

    public class CreateReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }

}
