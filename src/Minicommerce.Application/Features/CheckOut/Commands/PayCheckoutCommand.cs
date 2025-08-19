using MediatR;
using Minicommerce.Application.Checkout.Dtos;

namespace Minicommerce.Application.Checkout.Pay;

public sealed record PayCheckoutCommand(Guid CheckoutId, string PaymentMethod, string? TransactionId = null)
    : IRequest<CheckoutDto>;
