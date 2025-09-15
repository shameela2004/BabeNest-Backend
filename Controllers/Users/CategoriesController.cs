using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound(ApiResponse<object>.FailResponse("No products", statusCode: 404));
            }
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) 
                return NotFound(ApiResponse<object>.FailResponse("Category not found", statusCode: 404));

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok (ApiResponse<CategoryDto>.SuccessResponse(categoryDto));
        }
    }
}
