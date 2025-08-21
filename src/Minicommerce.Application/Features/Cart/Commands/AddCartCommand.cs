// Minicommerce.Application.Cart.AddItem/AddToCartCommand.cs
using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Cart.AddItem;

public sealed record AddToCartCommand(Guid ProductId, int Quantity) : IRequest<Result<CartDto>>;
