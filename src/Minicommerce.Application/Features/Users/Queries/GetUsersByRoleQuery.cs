using System;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;

namespace Minicommerce.Application.Features.Users.Queries;

public class GetUsersByRoleQuery : IRequest<IEnumerable<UserListDto>>
{
    public string Role { get; set; } = string.Empty;
}