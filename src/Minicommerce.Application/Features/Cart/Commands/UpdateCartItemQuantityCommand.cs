// Minicommerce.Application.Cart.UpdateQuantity/UpdateCartItemQuantityCommand.cs
using MediatR;

namespace Minicommerce.Application.Cart.UpdateQuantity;

public sealed record UpdateCartItemQuantityCommand(Guid ProductId, int Quantity) : IRequest<CartDto>;
