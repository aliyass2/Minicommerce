using System;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;

namespace Minicommerce.Application.Features.Users.Queries;

public class GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>
{
}

