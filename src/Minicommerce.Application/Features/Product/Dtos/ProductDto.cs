namespace Minicommerce.Application.Catalog.Products.Models;

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public int StockQuantity { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = default!;
}
public sealed class PatchProductDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? Currency { get; init; }
    public int? StockQuantity { get; init; }
    public Guid? CategoryId { get; init; }
}
