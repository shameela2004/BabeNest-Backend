using BabeNest_Backend.Entities;

namespace BabeNest_Backend.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Blocked { get; set; }
    }
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class UpdateUserDto
    {
        public string Username { get; set; }
        // For password update
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        //public string Email { get; set; }
        //public string Role { get; set; }
        //public bool Blocked { get; set; }
    }
    public  class AdminUserProfileDto
    {
        public UserDto User { get; set; }
        public IEnumerable<CartDto> Carts { get; set; } = Enumerable.Empty<CartDto>();
        public IEnumerable<WishlistDto> Wishlists { get; set; } = Enumerable.Empty<WishlistDto>();
        public IEnumerable<OrderDto> Orders { get; set; } = Enumerable.Empty<OrderDto>();
    }

    public class UserFilterDto
    {
        public string? Search { get; set; } // search by name or email
        public string? SortBy { get; set; } // "nameAsc", "nameDesc", "blockedFirst"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
