using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Catalog.Products.Models;

namespace Minicommerce.Application.Catalog.Products.List;

public sealed record ListProductsQuery : IRequest<Result<PaginatedList<ProductDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public Guid? CategoryId { get; init; }
    public string? Sort { get; init; } // "price_asc","price_desc","name","name_desc"
}
