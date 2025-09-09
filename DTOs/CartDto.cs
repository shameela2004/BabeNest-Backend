﻿namespace BabeNest_Backend.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateCartDto
    {
        //public int UserId { get; set; }
        public int ProductId { get; set; }
        //public int Quantity { get; set; }
    }

    public class UpdateCartDto
    {
        public int Quantity { get; set; }
    }

}
