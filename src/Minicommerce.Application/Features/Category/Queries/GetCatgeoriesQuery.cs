using System;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;

namespace Minicommerce.Application.Features.Category.Queries;

public sealed record GetCategoriesQuery : IRequest<Result<PaginatedList<CategoryDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string? Sort { get; init; } // "price_asc","price_desc","name","name_desc"
}
