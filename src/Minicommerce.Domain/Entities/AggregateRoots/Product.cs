using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Catalog;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public Money Price { get; private set; } = default!;
    public int StockQuantity { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;

    private Product() { } // EF Core

    public Product(string name, string description, Money price, int stockQuantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CatalogException("Product name cannot be empty.");

        if (price is null)
            throw new CatalogException("Product must have a price.");

        if (category is null)
            throw new CatalogException("Category is required.");

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
        CategoryId = category.Id;
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new CatalogException("Price must be greater than zero.");

        Price = newPrice;
        AddDomainEvent(new ProductPriceUpdatedEvent(Id, newPrice.Amount));
    }
public void UpdateName(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new CatalogException("Product name cannot be empty.");
    
    Name = name.Trim();
    AddDomainEvent(new ProductUpdatedEvent(Id, "Name", Name));
}

public void UpdateDescription(string description)
{
    Description = description?.Trim() ?? string.Empty;
    AddDomainEvent(new ProductUpdatedEvent(Id, "Description", Description));
}

public void UpdateStockQuantity(int stockQuantity)
{
    if (stockQuantity < 0)
        throw new CatalogException("Stock quantity cannot be negative.");
    
    var previousStock = StockQuantity;
    StockQuantity = stockQuantity;
    
    if (stockQuantity > previousStock)
    {
        AddDomainEvent(new ProductStockIncreasedEvent(Id, stockQuantity - previousStock));
    }
    else if (stockQuantity < previousStock)
    {
        AddDomainEvent(new ProductStockDecreasedEvent(Id, previousStock - stockQuantity));
    }
}

public void UpdateCategory(Category category)
{
    if (category is null)
        throw new CatalogException("Category is required.");
    
    Category = category;
    CategoryId = category.Id;
    AddDomainEvent(new ProductUpdatedEvent(Id, "Category", category.Name));
}
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new CatalogException("Quantity must be positive.");
        if (StockQuantity < quantity) throw new CatalogException("Not enough stock available.");

        StockQuantity -= quantity;
        AddDomainEvent(new ProductStockDecreasedEvent(Id, quantity));
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0) throw new CatalogException("Quantity must be positive.");

        StockQuantity += quantity;
        AddDomainEvent(new ProductStockIncreasedEvent(Id, quantity));
    }
}
