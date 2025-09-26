using AutoMapper;
using BabeNest_Backend.DTOs;
using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Mappings
{
    public class Mappingprofile : Profile
    {
        public Mappingprofile()
        {
            // Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
            
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            //Category 
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();


            // User
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // hash separately
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserProfileDto>();

            // Order
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<CreateOrderItemDto, OrderItem>();

            // Map Order → OrderDto
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.OrderStatus.Name))  //  map OrderStatus.Name
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src => src.PaymentStatus.Name)) // map PaymentStatus.Name
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.Name)) // map PaymentMethod.Name
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.Items));
            // Map CreateOrderDto → Order
            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.OrderStatusId,
                    opt => opt.MapFrom(src => 1)) // Default = Pending
                .ForMember(dest => dest.PaymentStatusId,
                    opt => opt.MapFrom(src => 1)); // Default = Pending


            // OrderItem 
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<CreateOrderItemDto, OrderItem>();
            CreateMap<UpdateOrderItemDto, OrderItem>();

            CreateMap<OrderItem, OrderItemDto>()
    .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Image))
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name));


            CreateMap<CreateOrderItemDto, OrderItem>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice));


            // Cart
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<CreateCartDto, Cart>();
            CreateMap<UpdateCartDto, Cart>();

            // Wishlist
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
    .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Image))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name));
            CreateMap<CreateWishlistDto, Wishlist>();

            // Review
            //CreateMap<Review, ReviewDto>()
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            //CreateMap<CreateReviewDto, Review>();
            // Entity -> DTO
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                  .ForMember(dest => dest.ReviewText, opt => opt.MapFrom(src => src.ReviewText))
    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
             .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
    .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Image));
            // src.UserName is already in entity, no need to map from User.Username

            // DTO -> Entity (Create)
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())   // will be set in service from JWT
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) // will be set in service from JWT
                .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.Now));

           

        }
    }
}
