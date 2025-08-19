using MediatR;

namespace Minicommerce.Application.Catalog.Products.Add;

public sealed record AddProductCommand(
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid CategoryId
) : IRequest<Minicommerce.Application.Catalog.Products.Models.ProductDto>;
