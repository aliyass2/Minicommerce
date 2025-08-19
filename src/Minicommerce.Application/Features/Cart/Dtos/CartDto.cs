using Minicommerce.Application.Features.Cart.Dtos;

public sealed class CartDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = default!;
    public decimal TotalPrice { get; init; }
    public IReadOnlyCollection<CartItemDto> Items { get; init; } = Array.Empty<CartItemDto>();
}