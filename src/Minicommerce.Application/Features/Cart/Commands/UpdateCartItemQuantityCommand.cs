using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Cart.UpdateQuantity;

public sealed record UpdateCartItemQuantityCommand(Guid ProductId, int Quantity)
    : IRequest<Result<CartDto>>;
