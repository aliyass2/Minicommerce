using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Application.Features.Users.Queries;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserListDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ApplicationUser> users;

        if (!string.IsNullOrEmpty(request.Role))
        {
            users = await _unitOfWork.Users.GetByRoleAsync(request.Role, cancellationToken);
        }
        else
        {
            users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        }

        // Apply filters
        if (request.IsActive.HasValue)
        {
            users = users.Where(u => u.IsActive == request.IsActive.Value);
        }

        var userListDtos = _mapper.Map<IEnumerable<UserListDto>>(users);

        // Add roles to each user
        var result = new List<UserListDto>();
        foreach (var userDto in userListDtos)
        {
            var user = users.First(u => u.Id == userDto.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var userListDto = userDto;
            userListDto.Roles = roles;
            result.Add(userListDto);
        }

        return result;
    }
}