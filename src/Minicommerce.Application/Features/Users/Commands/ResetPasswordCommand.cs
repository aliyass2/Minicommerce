using System;
using MediatR;

namespace Minicommerce.Application.Features.Users.Commands;

public class ResetPasswordCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
