using System;

namespace Minicommerce.Application.Features.Cart.Dtos;

public sealed class CartItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}
