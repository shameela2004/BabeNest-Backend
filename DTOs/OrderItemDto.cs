namespace BabeNest_Backend.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }   // for frontend
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; } // from Product.Category.Name

    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }   // for frontend
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductImage { get; set; }  //  add this
    }

    public class UpdateOrderItemDto
    {
        public int Quantity { get; set; }
    }
}
