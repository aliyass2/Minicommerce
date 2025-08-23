using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Cart.Clear;

public sealed record ClearCartCommand : IRequest<Result<CartDto>>;
