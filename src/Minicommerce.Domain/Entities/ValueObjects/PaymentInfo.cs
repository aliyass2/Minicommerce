using Minicommerce.Domain.Common;

namespace Minicommerce.Domain.Checkout;

public class PaymentInfo : ValueObject
{
    public string PaymentMethod { get; private set; } = default!;
    public string TransactionId { get; private set; } = default!;
    public bool IsMocked { get; private set; }

    private PaymentInfo() { } // EF Core

    public PaymentInfo(string paymentMethod, string transactionId, bool isMocked = true)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new CheckoutException("Payment method is required.");

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new CheckoutException("Transaction Id is required.");

        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
        IsMocked = isMocked;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PaymentMethod;
        yield return TransactionId;
        yield return IsMocked;
    }
}
