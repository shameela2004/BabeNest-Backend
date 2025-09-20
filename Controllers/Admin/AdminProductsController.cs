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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public AdminProductsController(IProductService productService , IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilter filters)
        {
            var (products, totalCount) = await _productService.GetAllAsync(filters);
            if(products== null) return NotFound(ApiResponse<object>.FailResponse("No products", statusCode: 404));
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            var result = new
            {
                Items = productDtos,
                TotalCount = totalCount,
                Page = filters.Page,
                PageSize = filters.PageSize
            };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Products retrieved successfully"));
        }
        // admin also can get details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound(ApiResponse<object>.FailResponse("Product Not Found"));
            var productDto = _mapper.Map<ProductDto>(product);

            return Ok(ApiResponse<ProductDto>.SuccessResponse(productDto,"Product retrieved successfully."));
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto request)
        {   
            if (request == null)
                return BadRequest(ApiResponse<object>.FailResponse("Invalid product data", statusCode: 400));
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId
            };

            var created = await _productService.CreateAsync(product, request.ImageFile);
            var createdDto = _mapper.Map<ProductDto>(created);

            return Ok(ApiResponse<ProductDto>.SuccessResponse(createdDto, "Product created successfully"));
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto request)
        {
            if (request == null)
                return BadRequest(ApiResponse<object>.FailResponse("Invalid product data", statusCode: 400));
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId
            };

            var updated = await _productService.UpdateAsync(id, product, request.ImageFile);
            if (updated == null)
                return NotFound(ApiResponse<object>.FailResponse("Product not found", statusCode: 404));

            var updatedDto = _mapper.Map<ProductDto>(updated);
            return Ok(ApiResponse<ProductDto>.SuccessResponse(updatedDto, "Product updated successfully"));
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success) return NotFound(ApiResponse<object>.FailResponse("Deletion failed.There is no such Product"));
            return Ok(ApiResponse<Object>.SuccessResponse("Product deleted Successfully."));
        }

    
    }
}
