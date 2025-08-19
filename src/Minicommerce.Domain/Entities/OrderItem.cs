using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Orders;

public class OrderItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem() { } // EF Core

    public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0) throw new OrderException("Quantity must be greater than zero.");
        if (unitPrice < 0) throw new OrderException("Unit price cannot be negative.");

        ProductId = productId;
        ProductName = productName ?? throw new OrderException("Product name is required.");
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
