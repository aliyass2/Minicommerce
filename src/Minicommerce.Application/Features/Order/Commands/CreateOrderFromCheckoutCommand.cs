// Minicommerce.Application.Orders.Create/CreateOrderFromCheckoutCommand.cs
using MediatR;
using Minicommerce.Application.Orders.Dtos;

namespace Minicommerce.Application.Orders.Create;

public sealed record CreateOrderFromCheckoutCommand(Guid CheckoutId) : IRequest<OrderDto>;
