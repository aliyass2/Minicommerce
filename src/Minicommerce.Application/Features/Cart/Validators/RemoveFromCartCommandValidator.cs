// Minicommerce.Application.Cart.RemoveItem/RemoveFromCartCommandValidator.cs
using FluentValidation;

namespace Minicommerce.Application.Cart.RemoveItem;

public sealed class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
