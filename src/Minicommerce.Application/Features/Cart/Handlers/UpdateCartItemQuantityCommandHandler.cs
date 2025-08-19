// Minicommerce.Application.Cart.UpdateQuantity/UpdateCartItemQuantityCommandHandler.cs
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Cart.UpdateQuantity;

public sealed class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, CartDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public UpdateCartItemQuantityCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(UpdateCartItemQuantityCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new CartException("User must be authenticated to modify a cart.");

        var cartRepo = _uow.Repository<Minicommerce.Domain.Cart.Cart>();
        var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (cart is null)
            throw new CartException("Cart not found.");

        if (request.Quantity == 0)
        {
            // Remove the item if quantity is 0
            cart.RemoveItem(request.ProductId);
        }
        else
        {
            // Update quantity (domain method throws if item not found / invalid)
            cart.UpdateQuantity(request.ProductId, request.Quantity);
        }

        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<CartDto>(cart);
    }
}
