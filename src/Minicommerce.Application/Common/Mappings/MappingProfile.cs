
using AutoMapper;
using Minicommerce.Application.Catalog.Products.Models;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Features.Cart.Dtos;
using Minicommerce.Application.Features.Category.Dtos;
using Minicommerce.Application.Features.CheckOut.Dtos;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Application.Orders.Dtos;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Orders;

namespace Minicommerce.Application.Common.Mappings;

public class MappingProfile : Profile
{
    // Helper method for better status display names
    public MappingProfile()
    {
        // User Mappings
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<CreateUserDto, ApplicationUser>();
        CreateMap<UpdateUserDto, ApplicationUser>();
        CreateMap<ApplicationUser, UserListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        //Product Mapping
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Price, cfg => cfg.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, cfg => cfg.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.CategoryName, cfg => cfg.MapFrom(s => s.Category.Name));

        //Category Mapping
        CreateMap<Category, CategoryDto>();

        //Cart Mapping
        CreateMap<CartItem, CartItemDto>();
        CreateMap<Minicommerce.Domain.Cart.Cart, CartDto>()
            .ForMember(d => d.TotalPrice, cfg => cfg.MapFrom(s => s.TotalPrice))
            .ForMember(d => d.Items, cfg => cfg.MapFrom(s => s.Items));
        //Checkout Mapping
        CreateMap<CheckoutItem, CheckoutItemDto>();

        CreateMap<Domain.Checkout.Checkout, CheckoutDto>()
            .ForMember(d => d.Status, cfg => cfg.MapFrom(s => s.Status.ToString()))
            // .ForMember(d => d.PaymentMethod, cfg => cfg.MapFrom(s => s.Payment != null ? s.Payment.PaymentMethod : null))
            // .ForMember(d => d.TransactionId, cfg => cfg.MapFrom(s => s.Payment != null ? s.Payment.TransactionId : null))
            .ForMember(d => d.Items, cfg => cfg.MapFrom(s => s.Items));
        
        //Orders Mapping
        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<Domain.Orders.Order, OrderDto>()
            .ForMember(d => d.Status, cfg => cfg.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PaymentMethod, cfg => cfg.MapFrom(s => s.Payment.PaymentMethod))
            .ForMember(d => d.TransactionId, cfg => cfg.MapFrom(s => s.Payment.TransactionId))
            .ForMember(d => d.Items, cfg => cfg.MapFrom(s => s.Items));




    }
}

// Common DTOs for Lookups
public class LookupDto<T>
{
    public T Id { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}

// Specific Guid Lookup for cleaner usage
public class GuidLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}