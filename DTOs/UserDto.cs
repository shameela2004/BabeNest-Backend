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

    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        //public string Role { get; set; }
        //public bool Blocked { get; set; }
    }
}
