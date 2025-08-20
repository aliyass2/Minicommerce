using System;

namespace Minicommerce.Application.Features.Users.Dtos;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; } // Number of users with this role
}
