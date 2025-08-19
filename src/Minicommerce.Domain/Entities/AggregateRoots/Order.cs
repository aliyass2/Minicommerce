using Minicommerce.Domain.Common;
using Minicommerce.Domain.Checkout;
using CheckoutAggregate = Minicommerce.Domain.Checkout.Checkout;

namespace Minicommerce.Domain.Orders;

public class Order : AggregateRoot
{
    public string UserId { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Placed;
    public PaymentInfo Payment { get; private set; } = default!;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    private Order(string userId, IEnumerable<OrderItem> items, PaymentInfo? payment)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new OrderException("Order must be associated with a valid user.");
        if (items == null || !items.Any())
            throw new OrderException("Order requires at least one item.");
        if (payment is null)
            throw new OrderException("Payment information is required.");

        UserId = userId;
        _items.AddRange(items);
        Payment = payment;
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }

    // ✅ Factory: Create Order from Checkout
    public static Order FromCheckout(CheckoutAggregate checkout)
    {
        if (checkout == null) throw new OrderException("Checkout cannot be null.");
        if (checkout.Status != CheckoutStatus.Completed)
            throw new OrderException("Checkout must be completed before creating an order.");

        var items = checkout.Items.Select(i =>
            new OrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)
        ).ToList();

        return new Order(checkout.UserId, items, checkout.Payment!);
    }

    // Business flow
    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Placed)
            throw new OrderException("Only placed orders can be shipped.");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Id));
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new OrderException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
        AddDomainEvent(new OrderDeliveredEvent(Id));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new OrderException("Delivered orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }
}
