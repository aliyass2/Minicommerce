using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Checkout.Complete;

public sealed record CompleteCheckoutCommand(Guid CheckoutId) : IRequest<Result<CheckoutDto>>;
