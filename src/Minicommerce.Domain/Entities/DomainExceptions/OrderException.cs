using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Orders;

public class OrderException : DomainException
{
    public OrderException(string message) : base(message) { }
    public OrderException(string message, Exception inner) : base(message, inner) { }
}
