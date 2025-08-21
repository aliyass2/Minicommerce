using System;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Orders.Dtos;

namespace Minicommerce.Application.Features.Order.Queries;

public sealed record ListOrdersQuery : IRequest<Result<PaginatedList<OrderDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Sort { get; init; } // "total_asc","total_desc","status","status_desc"
    // Optional admin filter (if you want to allow filtering by user)
    public string? UserId { get; init; }
}