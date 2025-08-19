using FluentValidation;

namespace Minicommerce.Application.Checkout.Complete;

public sealed class CompleteCheckoutCommandValidator : AbstractValidator<CompleteCheckoutCommand>
{
    public CompleteCheckoutCommandValidator()
    {
        RuleFor(x => x.CheckoutId).NotEmpty();
    }
}
