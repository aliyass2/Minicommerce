using Minicommerce.Application.Features.CheckOut.Dtos;

namespace Minicommerce.Application.Checkout.Dtos;

public sealed class CheckoutDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = default!; // string for client readability
    public string? PaymentMethod { get; init; }
    public string? TransactionId { get; init; }
    public IReadOnlyCollection<CheckoutItemDto> Items { get; init; } = Array.Empty<CheckoutItemDto>();
}