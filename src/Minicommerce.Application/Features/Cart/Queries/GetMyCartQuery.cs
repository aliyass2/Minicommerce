using System;
using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Features.Cart.Queries;

public sealed record GetMyCartQuery(string UserId) : IRequest<Result<CartDto>>;
