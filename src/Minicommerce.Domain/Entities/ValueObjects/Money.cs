using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Catalog;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";

    private Money() { } // EF Core

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new CatalogException("Amount cannot be negative.");
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new CatalogException("Currency must be provided.");

        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
