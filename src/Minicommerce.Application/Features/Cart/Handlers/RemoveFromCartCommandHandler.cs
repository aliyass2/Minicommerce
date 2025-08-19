// Minicommerce.Application.Cart.RemoveItem/RemoveFromCartCommandHandler.cs
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Cart.RemoveItem;

public sealed class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, CartDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public RemoveFromCartCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(RemoveFromCartCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new CartException("User must be authenticated to modify a cart.");

        var cartRepo = _uow.Repository<Minicommerce.Domain.Cart.Cart>();

        var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (cart is null)
            throw new CartException("Cart not found.");

        // DDD method throws if item not found (as we designed)
        cart.RemoveItem(request.ProductId);

        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<CartDto>(cart);
    }
}
