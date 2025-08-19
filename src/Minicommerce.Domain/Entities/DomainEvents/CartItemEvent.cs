using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Cart;

public class CartItemAddedEvent : DomainEvent
{
    public Guid CartId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }

    public CartItemAddedEvent(Guid cartId, Guid productId, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }
}

public class CartItemRemovedEvent : DomainEvent
{
    public Guid CartId { get; }
    public Guid ProductId { get; }

    public CartItemRemovedEvent(Guid cartId, Guid productId)
    {
        CartId = cartId;
        ProductId = productId;
    }
}

public class CartItemQuantityUpdatedEvent : DomainEvent
{
    public Guid CartId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }

    public CartItemQuantityUpdatedEvent(Guid cartId, Guid productId, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }
}

public class CartClearedEvent : DomainEvent
{
    public Guid CartId { get; }

    public CartClearedEvent(Guid cartId)
    {
        CartId = cartId;
    }
}
