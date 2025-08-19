using System;

namespace Minicommerce.Application.Features.CheckOut.Dtos;

public sealed class CheckoutItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}
