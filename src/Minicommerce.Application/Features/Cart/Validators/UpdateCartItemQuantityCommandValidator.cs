// Minicommerce.Application.Cart.UpdateQuantity/UpdateCartItemQuantityCommandValidator.cs
using FluentValidation;

namespace Minicommerce.Application.Cart.UpdateQuantity;

public sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be >= 0. Use 0 to remove the item.");
    }
}
