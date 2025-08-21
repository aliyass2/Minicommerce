using System;
using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Features.CheckOut.Queries;

public sealed record GetMyCheckoutQuery(string UserId) : IRequest<Result<PaginatedList<CheckoutDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Sort { get; init; } // "total_asc","total_desc","status","status_desc"
}
