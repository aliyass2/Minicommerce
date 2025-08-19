using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Orders;

public class OrderShippedEvent : DomainEvent
{
    public Guid OrderId { get; }

    public OrderShippedEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}

public class OrderDeliveredEvent : DomainEvent
{
    public Guid OrderId { get; }

    public OrderDeliveredEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}

public class OrderCancelledEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string Reason { get; }

    public OrderCancelledEvent(Guid orderId, string reason)
    {
        OrderId = orderId;
        Reason = reason;
    }
}
