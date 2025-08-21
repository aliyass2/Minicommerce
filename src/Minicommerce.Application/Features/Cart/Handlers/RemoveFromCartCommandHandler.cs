using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Cart.RemoveItem;

public sealed class RemoveFromCartCommandHandler
    : IRequestHandler<RemoveFromCartCommand, Result<CartDto>>
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

    public async Task<Result<CartDto>> Handle(RemoveFromCartCommand request, CancellationToken ct)
    {
        try
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                throw new CartException("User must be authenticated to modify a cart.");

            var cartRepo = _uow.Repository<Minicommerce.Domain.Cart.Cart>();

            var cart = await cartRepo
                .GetQueryable()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart is null)
                throw new CartException("Cart not found.");

            if (!cart.Items.Any(i => i.ProductId == request.ProductId))
                throw new CartException("Item not found in cart.");

            cart.RemoveItem(request.ProductId);

            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<CartDto>(cart);
            return Result<CartDto>.Success(dto);
        }
        catch (CartException ex)
        {
            return Result<CartDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<CartDto>.Failure($"Failed to remove item from cart: {ex.Message}");
        }
    }
}
