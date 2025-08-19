using Minicommerce.Domain.Common;
using Minicommerce.Domain.Cart; // reference Cart domain

namespace Minicommerce.Domain.Checkout;

public class Checkout : AggregateRoot
{
    public string UserId { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    public CheckoutStatus Status { get; private set; } = CheckoutStatus.Pending;
    public PaymentInfo? Payment { get; private set; }

    private readonly List<CheckoutItem> _items = new();
    public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

    private Checkout() { } // EF Core

    public Checkout(string userId, IEnumerable<CheckoutItem> items)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new CheckoutException("Checkout must be associated with a valid user.");

        if (items == null || !items.Any())
            throw new CheckoutException("Checkout requires at least one item.");

        UserId = userId;
        _items.AddRange(items);
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }

    // ✅ Factory: Build Checkout from Cart
    public static Checkout FromCart(Cart.Cart cart)
    {
        if (cart == null) throw new CheckoutException("Cart cannot be null.");
        if (!cart.Items.Any()) throw new CheckoutException("Cannot checkout with an empty cart.");

        var items = cart.Items.Select(i =>
            new CheckoutItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)
        ).ToList();

        return new Checkout(cart.UserId, items);
    }

    // Payment / Lifecycle
    public void MakePayment(PaymentInfo paymentInfo)
    {
        if (Status != CheckoutStatus.Pending)
            throw new CheckoutException("Payment can only be processed for pending checkouts.");

        Payment = paymentInfo;
        Status = CheckoutStatus.Paid;
        AddDomainEvent(new CheckoutPaidEvent(Id, paymentInfo.TransactionId));
    }

    public void MarkAsFailed(string reason)
    {
        if (Status != CheckoutStatus.Pending)
            throw new CheckoutException("Only pending checkouts can be marked as failed.");

        Status = CheckoutStatus.Failed;
        AddDomainEvent(new CheckoutFailedEvent(Id, reason));
    }

    public void Complete()
    {
        if (Status != CheckoutStatus.Paid)
            throw new CheckoutException("Only paid checkouts can be completed.");

        Status = CheckoutStatus.Completed;
        AddDomainEvent(new CheckoutCompletedEvent(Id));
    }
}
