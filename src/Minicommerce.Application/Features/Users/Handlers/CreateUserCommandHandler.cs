using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<ApplicationUser>(request.CreateUserDto);
        user.CreatedAt = DateTime.UtcNow;

        var result = await _userManager.CreateAsync(user, request.CreateUserDto.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user: {errors}");
        }
        if (string.IsNullOrEmpty(request.CreateUserDto.Position))
        {
            request.CreateUserDto.Position = "Customer";
        }

        // Add roles to user
        if (request.CreateUserDto.Roles.Any())
        {
            var roleResult = await _userManager.AddToRolesAsync(user, request.CreateUserDto.Roles);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to assign roles: {errors}");
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = await _userManager.GetRolesAsync(user);
        
        return userDto;
    }
}