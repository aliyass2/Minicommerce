using FluentValidation;

namespace Minicommerce.Application.Checkout.Pay;

public sealed class PayCheckoutCommandValidator : AbstractValidator<PayCheckoutCommand>
{
    public PayCheckoutCommandValidator()
    {
        RuleFor(x => x.CheckoutId).NotEmpty();
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TransactionId).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.TransactionId));
    }
}
