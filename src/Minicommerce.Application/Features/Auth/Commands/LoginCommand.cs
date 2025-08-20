using System;
using MediatR;
using Minicommerce.Application.Features.Auth.Dtos;

namespace Minicommerce.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<LoginResponseDto?>
{
    public LoginDto LoginDto { get; set; } = new();
}
