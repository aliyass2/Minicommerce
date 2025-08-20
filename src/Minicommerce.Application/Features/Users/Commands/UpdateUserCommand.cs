using System;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;

namespace Minicommerce.Application.Features.Users.Commands;

public class UpdateUserCommand : IRequest<UserDto>
{
    public string Id { get; set; } = string.Empty;
    public UpdateUserDto UpdateUserDto { get; set; } = new();
}
