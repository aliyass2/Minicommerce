using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Cart;

public class Cart : AggregateRoot
{
    public string UserId { get; private set; } = default!;
    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { } // EF Core

    public Cart(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new CartException("Cart must be associated with a valid user.");

        UserId = userId;
    }

    // Business Logic
    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity = 1)
    {
        if (quantity <= 0)
            throw new CartException("Quantity must be greater than zero.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
        }
        else
        {
            var item = new CartItem(productId, productName, unitPrice, quantity);
            _items.Add(item);
        }

        AddDomainEvent(new CartItemAddedEvent(Id, productId, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null)
            throw new CartException("Item not found in cart.");

        _items.Remove(existing);
        AddDomainEvent(new CartItemRemovedEvent(Id, productId));
    }

    public void UpdateQuantity(Guid productId, int newQuantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null)
            throw new CartException("Item not found in cart.");

        if (newQuantity <= 0)
        {
            RemoveItem(productId);
        }
        else
        {
            existing.UpdateQuantity(newQuantity);
            AddDomainEvent(new CartItemQuantityUpdatedEvent(Id, productId, newQuantity));
        }
    }

    // public void Clear()
    // {
    //     _items.Clear();
    //     AddDomainEvent(new CartClearedEvent(Id));
    // }
        public void Clear()
    {
        if (_items.Count == 0) return;        
        _items.Clear();                           
        AddDomainEvent(new CartClearedEvent(Id)); 
    }

    public decimal TotalPrice => _items.Sum(i => i.TotalPrice);
}
