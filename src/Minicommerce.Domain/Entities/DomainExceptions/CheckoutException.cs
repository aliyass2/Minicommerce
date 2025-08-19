using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Checkout;

public class CheckoutException : DomainException
{
    public CheckoutException(string message) : base(message) { }
    public CheckoutException(string message, Exception inner) : base(message, inner) { }
}
