using AutoMapper;
using BabeNest_Backend.Data;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabeNest_Backend.Services
{
    public class AddressService : IAddressService
    {
        private readonly BabeNestDbContext _context;
        private readonly IMapper _mapper;

        public AddressService(BabeNestDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAsync(int userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto> GetByIdAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);
            return _mapper.Map<AddressDto>(address);
        }

        public async Task<AddressDto> CreateAsync(int userId, CreateAddressDto dto)
        {
            var address = _mapper.Map<Address>(dto);
            address.UserId = userId;

            // If first address, set as default
            if (!await _context.Addresses.AnyAsync(a => a.UserId == userId))
                address.IsDefault = true;

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDto>(address);
        }

        public async Task<AddressDto> UpdateAsync(int userId, int addressId, UpdateAddressDto dto)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);

            if (address == null) return null;

            _mapper.Map(dto, address);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDto>(address);
        }

        public async Task<bool> DeleteAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);

            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAsync(int userId, int addressId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var address = addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null) return false;

            // Reset all others
            foreach (var addr in addresses)
                addr.IsDefault = false;

            address.IsDefault = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
