using System;
using MediatR;

namespace Minicommerce.Application.Features.Users.Commands;

public class DeleteUserCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;
}
