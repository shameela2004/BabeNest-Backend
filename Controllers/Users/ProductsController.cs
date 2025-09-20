using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;
using BabeNest_Backend.Helpers;
using BabeNest_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BabeNest_Backend.Controllers.Users
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilter filters)
        {
            var (products,totalCount) = await _productService.GetAllAsync(filters);
            if (products == null || !products.Any())
            {
                return NotFound(ApiResponse<object>.FailResponse("No products", statusCode: 404));
            }
            var productdtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            var result = new
            {
                Items = productdtos,
                TotalCount = totalCount,
                Page = filters.Page,
                PageSize = filters.PageSize
            };
            return Ok(ApiResponse<object>.SuccessResponse(result, "Products retrieved successfully"));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<object>.FailResponse("Product Not Found ", statusCode: 404));

            var productDto = _mapper.Map<ProductDto>(product);

            return Ok( ApiResponse<ProductDto>.SuccessResponse(productDto, "Product retrieved successfully"));
        }
    }
}
