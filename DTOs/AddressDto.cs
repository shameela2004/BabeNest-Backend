namespace BabeNest_Backend.DTOs
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateAddressDto
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class UpdateAddressDto
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}
