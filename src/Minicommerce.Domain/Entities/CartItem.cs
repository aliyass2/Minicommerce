using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Cart;

public class CartItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    private CartItem() { } // EF Core

    public CartItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new CartException("Quantity must be greater than zero.");
        if (unitPrice < 0)
            throw new CartException("Unit price cannot be negative.");

        ProductId = productId;
        ProductName = productName ?? throw new CartException("Product name is required.");
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new CartException("Quantity must be positive.");

        Quantity += quantity;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new CartException("Quantity must be positive.");

        Quantity = newQuantity;
    }
}
