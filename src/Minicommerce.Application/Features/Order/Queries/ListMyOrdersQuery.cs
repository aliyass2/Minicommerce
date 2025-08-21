using System;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Orders.Dtos;

namespace Minicommerce.Application.Features.Order.Queries;

public sealed record ListMyOrdersQuery(string UserId) : IRequest<Result<PaginatedList<OrderDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Sort { get; init; } // "total_asc","total_desc","status","status_desc"
}
