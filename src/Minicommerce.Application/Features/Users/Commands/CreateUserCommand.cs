using System;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;

namespace Minicommerce.Application.Features.Users.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public CreateUserDto CreateUserDto { get; set; } = new();
}