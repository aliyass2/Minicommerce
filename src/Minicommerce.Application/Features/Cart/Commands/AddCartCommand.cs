// Minicommerce.Application.Cart.AddItem/AddToCartCommand.cs
using MediatR;

namespace Minicommerce.Application.Cart.AddItem;

public sealed record AddToCartCommand(Guid ProductId, int Quantity) : IRequest<CartDto>;
