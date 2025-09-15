//using BabeNest_Backend.DTOs;
//using BabeNest_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace BabeNest_Backend.Controllers.Users
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AddressController : ControllerBase
//    {
//        private readonly IAddressService _addressService;

//        public AddressController(IAddressService addressService)
//        {
//            _addressService = addressService;
//        }

//        // GET: api/address
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            //var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
//            var userIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
//            int.TryParse(userIdentifier, out int userId);
//            var addresses = await _addressService.GetAllAsync(userId);
//            return Ok(addresses);
//        }

//        // GET: api/address/5
//        [HttpGet("{id}")]
//        public async Task<IActionResult> Get(int id)
//        {
//            var userId = int.Parse(User.Identity.Name);
//            var address = await _addressService.GetByIdAsync(userId, id);

//            if (address == null) return NotFound();
//            return Ok(address);
//        }

//        // POST: api/address
//        [HttpPost]
//        public async Task<IActionResult> Create(CreateAddressDto dto)
//        {
//            var userId = int.Parse(User.Identity.Name);
//            var address = await _addressService.CreateAsync(userId, dto);
//            return CreatedAtAction(nameof(Get), new { id = address.Id }, address);
//        }

//        // PUT: api/address/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, UpdateAddressDto dto)
//        {
//            var userId = int.Parse(User.Identity.Name);
//            var updated = await _addressService.UpdateAsync(userId, id, dto);

//            if (updated == null) return NotFound();
//            return Ok(updated);
//        }

//        // DELETE: api/address/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var userId = int.Parse(User.Identity.Name);
//            var deleted = await _addressService.DeleteAsync(userId, id);

//            if (!deleted) return NotFound();
//            return NoContent();
//        }

//        // PATCH: api/address/default/5
//        [HttpPatch("default/{id}")]
//        public async Task<IActionResult> SetDefault(int id)
//        {
//            var userId = int.Parse(User.Identity.Name);
//            var result = await _addressService.SetDefaultAsync(userId, id);

//            if (!result) return NotFound();
//            return NoContent();
//        }
//    }
//}
