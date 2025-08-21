using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Checkout.Pay;

public sealed record PayCheckoutCommand(Guid CheckoutId, string PaymentMethod, string? TransactionId = null)
    : IRequest<Result<CheckoutDto>>;

