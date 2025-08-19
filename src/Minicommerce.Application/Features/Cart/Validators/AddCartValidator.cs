// Minicommerce.Application.Cart.AddItem/AddToCartCommandValidator.cs
using FluentValidation;

namespace Minicommerce.Application.Cart.AddItem;

public sealed class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}
