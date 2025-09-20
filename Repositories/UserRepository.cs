using BabeNest_Backend.Data;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Repositories
{
    public class UserRepository:IUserRepository

    {
        private readonly BabeNestDbContext _context;

        public UserRepository(BabeNestDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<(IEnumerable<User>, int)> GetFilteredUsersAsync(UserFilterDto query)
        {
            var usersQuery = _context.Users.AsQueryable();

            // Filtering / Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Username.Contains(query.Search) ||
                    u.Email.Contains(query.Search));
            }

            // Sorting
            usersQuery = query.SortBy switch
            {
                "nameAsc" => usersQuery.OrderBy(u => u.Username),
                "nameDesc" => usersQuery.OrderByDescending(u => u.Username),
                "blockedFirst" => usersQuery.OrderByDescending(u => u.Blocked),
                _ => usersQuery.OrderBy(u => u.Id)
            };

            // Total count before pagination
            var totalCount = await usersQuery.CountAsync();

            // Pagination
            var users = await usersQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (users, totalCount);
        }


        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
