using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Application.Features.Users.Queries;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = await _userManager.GetRolesAsync(user);
        
        return userDto;
    }
}