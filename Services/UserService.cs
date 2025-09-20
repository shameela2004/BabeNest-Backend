using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using BabeNest_Backend.Services.Interfaces;

namespace BabeNest_Backend.Services
{
    public class UserService :IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepo;
        private readonly IwishlistRepository _wishlistRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper, ICartRepository cartRepo, IwishlistRepository wishlistRepo, IOrderRepository orderRepo)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _cartRepo = cartRepo;
            _wishlistRepo = wishlistRepo;
            _orderRepo = orderRepo;
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto dto, string passwordHash)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = passwordHash;
            user.Role = "User";
            user.Blocked = false;
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserProfileDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserProfileDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<(IEnumerable<UserDto>, int)> GetUsersAsync(UserFilterDto query)
        {
            var (users, totalCount) = await _userRepository.GetFilteredUsersAsync(query);
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return (usersDto, totalCount);
        }


        public async Task<UserProfileDto?> UpdateAsync(int id, UpdateUserDto dto)
        {
            //var user = await _userRepository.GetByIdAsync(id);
            //if (user == null) return null;

            //_mapper.Map(dto, user); // updates fields directly

            //await _userRepository.UpdateAsync(user);

            //return _mapper.Map<UserProfileDto>(user);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // Update profile fields
            user.Username = dto.Username ?? user.Username;

            // Handle password update (only if provided)
            if (!string.IsNullOrEmpty(dto.OldPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                // Verify old password
                if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Old password is incorrect.");
                }

                // Hash new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserProfileDto>(user);


        }



        // Get single user's profile with cart, wishlist, orders
        public async Task<AdminUserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null; // avoid mapping null user

            var carts = await _cartRepo.GetUserCartAsync(userId) ?? new List<Cart>();
            var wishlists = await _wishlistRepo.GetUserWishlistAsync(userId) ?? new List<Wishlist>();
            var orders = await _orderRepo.GetOrdersByUserAsync(userId) ?? new List<Order>();

            return new AdminUserProfileDto
            {
                User = _mapper.Map<UserDto>(user),
                Carts = _mapper.Map<IEnumerable<CartDto>>(carts),      
                Wishlists = _mapper.Map<IEnumerable<WishlistDto>>(wishlists),
                Orders = _mapper.Map<IEnumerable<OrderDto>>(orders)
            };
        }

        public async Task<bool> BlockUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Blocked = true;
            await _userRepository.UpdateAsync(user);

            return true;
        }
        public async Task<bool> UnblockUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Blocked = false; 
            await _userRepository.UpdateAsync(user);
            return true;
        }

    }
}
