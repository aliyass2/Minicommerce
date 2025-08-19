// Minicommerce.Application.Cart.RemoveItem/RemoveFromCartCommand.cs
using MediatR;

namespace Minicommerce.Application.Cart.RemoveItem;

public sealed record RemoveFromCartCommand(Guid ProductId) : IRequest<CartDto>;
