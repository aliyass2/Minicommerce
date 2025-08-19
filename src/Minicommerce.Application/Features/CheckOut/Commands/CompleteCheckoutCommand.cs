using MediatR;
using Minicommerce.Application.Checkout.Dtos;

namespace Minicommerce.Application.Checkout.Complete;

public sealed record CompleteCheckoutCommand(Guid CheckoutId) : IRequest<CheckoutDto>;
