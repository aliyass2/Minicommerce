// Minicommerce.Application.Cart.Clear/ClearCartCommandHandler.cs
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Cart.Clear;

public sealed class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, CartDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public ClearCartCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(ClearCartCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new CartException("User must be authenticated to modify a cart.");

        var cartRepo = _uow.Repository<Minicommerce.Domain.Cart.Cart>();
        var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (cart is null)
            throw new CartException("Cart not found.");

        cart.Clear();

        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<CartDto>(cart);
    }
}
