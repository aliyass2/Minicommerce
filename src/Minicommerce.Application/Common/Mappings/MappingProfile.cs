
using AutoMapper;

namespace Minicommerce.Application.Common.Mappings;

public class MappingProfile : Profile
{
    // Helper method for better status display names
    public MappingProfile()
    {
        // User Mappings
        // CreateMap<ApplicationUser, UserDto>();
        // CreateMap<CreateUserDto, ApplicationUser>();
        // CreateMap<UpdateUserDto, ApplicationUser>();
        // CreateMap<ApplicationUser, UserListDto>()
        //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
        //     .ForMember(dest => dest.Roles, opt => opt.Ignore());

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