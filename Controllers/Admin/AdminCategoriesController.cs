using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public AdminCategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            if (categories == null || !categories.Any()) 
                return NotFound(ApiResponse<Object>.FailResponse("There are no categories"));
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categoryDtos,""));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound(ApiResponse<Object>.FailResponse("There is no Category"));

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(categoryDto,"Category retrieved successfully.."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createDto)
        {
            if(createDto== null) return BadRequest(ApiResponse<Object>.FailResponse("Invalid category data"));
            var category = _mapper.Map<Category>(createDto);
            await _categoryService.CreateAsync(category);

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id },ApiResponse<CategoryDto>.SuccessResponse(categoryDto,"Category added successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto updateDto)
        {
            if (updateDto == null) return BadRequest(ApiResponse<Object>.FailResponse("Invalid category data"));
            var updatedCategory = _mapper.Map<Category>(updateDto);
            var category = await _categoryService.UpdateAsync(id, updatedCategory);

            if (category == null) return NotFound(ApiResponse<Object>.FailResponse("Category not found"));

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(categoryDto,"Category updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success) return NotFound(ApiResponse<Object>.FailResponse("There is no such category. Deletion failed."));

            return Ok(ApiResponse<Object>.SuccessResponse("Category deleted Successfully"));
        }
    }
}
