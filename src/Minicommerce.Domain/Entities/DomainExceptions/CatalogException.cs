using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Catalog;

public class CatalogException : DomainException
{
    public CatalogException(string message) : base(message) { }
    public CatalogException(string message, Exception inner) : base(message, inner) { }
}
