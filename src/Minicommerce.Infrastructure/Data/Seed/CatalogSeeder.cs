using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minicommerce.Domain.Catalog;
using Minicommerce.Infrastructure.Data;

namespace Minicommerce.Infrastructure.Data.Seed;

public class CatalogSeeder
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CatalogSeeder> _logger;

    public CatalogSeeder(ApplicationDbContext db, ILogger<CatalogSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // 1) Categories (idempotent by Name)
        var categoryNames = new[] { "Electronics", "Clothing", "Books", "Home & Kitchen" };
        var existing = await _db.Categories.Select(c => c.Name).ToListAsync(ct);
        var toAdd = categoryNames.Except(existing, StringComparer.OrdinalIgnoreCase)
                                 .Select(n => new Category(n))
                                 .ToList();

        if (toAdd.Count > 0)
        {
            _db.Categories.AddRange(toAdd);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seeded {Count} categories.", toAdd.Count);
        }

        // Fetch tracked categories
        var categories = await _db.Categories.ToDictionaryAsync(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase, ct);

        // 2) Products (idempotent by Name)
        var seedProducts = new (string Name, string Desc, Money Price, int Stock, string Cat)[]
        {
            ("Laptop Pro 14", "Slim 14” productivity laptop", new Money(1299.00m, "USD"), 25, "Electronics"),
            ("Wireless Earbuds", "Noise-cancelling true wireless earbuds", new Money(149.99m, "USD"), 100, "Electronics"),
            ("Cotton T-Shirt", "100% cotton, classic fit", new Money(19.99m, "USD"), 250, "Clothing"),
            ("Non-stick Frying Pan", "28cm non-stick pan", new Money(29.90m, "USD"), 80, "Home & Kitchen"),
            ("Novel – The Wanderer", "Bestselling contemporary fiction", new Money(12.50m, "USD"), 60, "Books")
        };

        // Get existing product names to avoid duplicates
        var existingProductNames = await _db.Products.Select(p => p.Name).ToListAsync(ct);

        var newProducts = new List<Product>();
        foreach (var p in seedProducts)
        {
            if (existingProductNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                continue;

            if (!categories.TryGetValue(p.Cat, out var cat))
            {
                _logger.LogWarning("Category {Category} missing; skipping product {Product}.", p.Cat, p.Name);
                continue;
            }

            // Use domain constructor (enforces invariants + raises events)
            var prod = new Product(p.Name, p.Desc, p.Price, p.Stock, cat);
            newProducts.Add(prod);
        }

        if (newProducts.Count > 0)
        {
            _db.Products.AddRange(newProducts);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seeded {Count} products.", newProducts.Count);
        }
    }
}
