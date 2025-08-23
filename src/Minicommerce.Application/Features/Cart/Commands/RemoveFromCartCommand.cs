using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Cart.RemoveItem;

public sealed record RemoveFromCartCommand(Guid ProductId) : IRequest<Result<CartDto>>;
