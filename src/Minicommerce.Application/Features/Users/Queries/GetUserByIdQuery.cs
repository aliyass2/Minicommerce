using System;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;

namespace Minicommerce.Application.Features.Users.Queries;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public string Id { get; set; } = string.Empty;
}
