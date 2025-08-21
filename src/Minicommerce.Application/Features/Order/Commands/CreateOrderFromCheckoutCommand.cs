using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Orders.Dtos;

namespace Minicommerce.Application.Orders.Create;

public sealed record CreateOrderFromCheckoutCommand(Guid CheckoutId)
    : IRequest<Result<OrderDto>>;
