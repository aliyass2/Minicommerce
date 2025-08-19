// Minicommerce.Application.Cart.Clear/ClearCartCommand.cs
using MediatR;

namespace Minicommerce.Application.Cart.Clear;

public sealed record ClearCartCommand : IRequest<CartDto>;
