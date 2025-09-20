namespace BabeNest_Backend.DTOs
{
    public class VerifyPaymentDto
    {
        public int OrderId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }
}
