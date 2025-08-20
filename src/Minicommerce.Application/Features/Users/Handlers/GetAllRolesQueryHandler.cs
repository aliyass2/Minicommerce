using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Application.Features.Users.Queries;
using Minicommerce.Domain.Entities.User;

namespace Minicommerce.Application.Features.Users.Handlers;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
        
        var roleDtos = new List<RoleDto>();
        
        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            
            roleDtos.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                UserCount = usersInRole.Count
            });
        }
        
        return roleDtos.OrderBy(r => r.Name);
    }
}