using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Checkout.Create;

public sealed record CreateCheckoutFromCartCommand() : IRequest<Result<CheckoutDto>>;
