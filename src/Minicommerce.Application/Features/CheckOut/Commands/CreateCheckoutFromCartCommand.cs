using MediatR;
using Minicommerce.Application.Checkout.Dtos;

namespace Minicommerce.Application.Checkout.Create;

public sealed record CreateCheckoutFromCartCommand() : IRequest<CheckoutDto>;
