using System;

namespace Minicommerce.Application.Orders.Dtos;

public sealed class OrderDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = default!;
    public string PaymentMethod { get; init; } = default!;
    public string TransactionId { get; init; } = default!;
    public IReadOnlyCollection<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();
}