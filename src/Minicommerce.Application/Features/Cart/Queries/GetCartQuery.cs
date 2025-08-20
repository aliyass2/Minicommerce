using System;
using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Features.Cart.Queries;

public sealed record GetCartQuery : IRequest<Result<PaginatedList<CartDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? UserId { get; init; }
    public string? Sort { get; init; }
}

