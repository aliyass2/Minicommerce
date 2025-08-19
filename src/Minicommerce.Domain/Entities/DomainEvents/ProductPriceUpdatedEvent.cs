using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Catalog;

public class ProductPriceUpdatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public decimal NewPrice { get; }

    public ProductPriceUpdatedEvent(Guid productId, decimal newPrice)
    {
        ProductId = productId;
        NewPrice = newPrice;
    }
}
    public class ProductUpdatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string PropertyName { get; }
        public object NewValue { get; }

        public ProductUpdatedEvent(Guid productId, string propertyName, object newValue)
        {
            ProductId = productId;
            PropertyName = propertyName;
            NewValue = newValue;
        }
    }
public class ProductStockDecreasedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int Quantity { get; }

    public ProductStockDecreasedEvent(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}

public class ProductStockIncreasedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int Quantity { get; }

    public ProductStockIncreasedEvent(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}
