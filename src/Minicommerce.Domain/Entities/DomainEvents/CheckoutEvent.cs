using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Checkout;

public class CheckoutPaidEvent : DomainEvent
{
    public Guid CheckoutId { get; }
    public string TransactionId { get; }

    public CheckoutPaidEvent(Guid checkoutId, string transactionId)
    {
        CheckoutId = checkoutId;
        TransactionId = transactionId;
    }
}

public class CheckoutFailedEvent : DomainEvent
{
    public Guid CheckoutId { get; }
    public string Reason { get; }

    public CheckoutFailedEvent(Guid checkoutId, string reason)
    {
        CheckoutId = checkoutId;
        Reason = reason;
    }
}

public class CheckoutCompletedEvent : DomainEvent
{
    public Guid CheckoutId { get; }

    public CheckoutCompletedEvent(Guid checkoutId)
    {
        CheckoutId = checkoutId;
    }
}
