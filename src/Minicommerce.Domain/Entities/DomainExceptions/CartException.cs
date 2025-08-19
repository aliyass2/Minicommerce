using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Cart;

public class CartException : DomainException
{
    public CartException(string message) : base(message) { }
    public CartException(string message, Exception inner) : base(message, inner) { }
}
