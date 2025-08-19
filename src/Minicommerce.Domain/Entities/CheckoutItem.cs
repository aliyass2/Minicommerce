using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Checkout;

public class CheckoutItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private CheckoutItem() { } // EF Core

    public CheckoutItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0) throw new CheckoutException("Quantity must be greater than zero.");
        if (unitPrice < 0) throw new CheckoutException("Unit price cannot be negative.");

        ProductId = productId;
        ProductName = productName ?? throw new CheckoutException("Product name is required.");
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
