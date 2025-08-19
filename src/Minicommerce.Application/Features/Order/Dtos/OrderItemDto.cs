namespace Minicommerce.Application.Orders.Dtos;

public sealed class OrderItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}