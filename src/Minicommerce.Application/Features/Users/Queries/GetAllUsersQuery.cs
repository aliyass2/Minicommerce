using System;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;

namespace Minicommerce.Application.Features.Users.Queries;

public class GetAllUsersQuery : IRequest<IEnumerable<UserListDto>>
{
    public bool? IsActive { get; set; }
    public string? Role { get; set; }
}
