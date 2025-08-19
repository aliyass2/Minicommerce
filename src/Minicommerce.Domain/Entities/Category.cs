using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Catalog;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;

    private Category() { } // EF Core

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CatalogException("Category name cannot be empty.");

        Name = name;
    }
}
