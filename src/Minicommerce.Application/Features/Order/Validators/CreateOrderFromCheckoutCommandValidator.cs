using FluentValidation;

namespace Minicommerce.Application.Orders.Create;

public sealed class CreateOrderFromCheckoutCommandValidator : AbstractValidator<CreateOrderFromCheckoutCommand>
{
    public CreateOrderFromCheckoutCommandValidator()
    {
        RuleFor(x => x.CheckoutId).NotEmpty();
    }
}
