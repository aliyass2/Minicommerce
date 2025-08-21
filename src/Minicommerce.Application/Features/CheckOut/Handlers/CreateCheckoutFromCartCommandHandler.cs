using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;                         // ← add
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Checkout.Create;

public sealed class CreateCheckoutFromCartCommandHandler
    : IRequestHandler<CreateCheckoutFromCartCommand, Result<CheckoutDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateCheckoutFromCartCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<CheckoutDto>> Handle(CreateCheckoutFromCartCommand request, CancellationToken ct)
    {
        try
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                throw new CheckoutException("User must be authenticated to start checkout.");

            var cartRepo     = _uow.Repository<Domain.Cart.Cart>();
            var checkoutRepo = _uow.Repository<Domain.Checkout.Checkout>();

            // Include items so we can validate non-empty cart
            var cart = await cartRepo
                .GetQueryable()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart is null)
                throw new CheckoutException("Cannot create checkout: cart not found for this user.");

            if (cart.Items is null || cart.Items.Count == 0)
                throw new CheckoutException("Cannot create checkout from an empty cart.");

            var checkout = Domain.Checkout.Checkout.FromCart(cart);
            await checkoutRepo.AddAsync(checkout, ct);
            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<CheckoutDto>(checkout);
            return Result<CheckoutDto>.Success(dto);
        }
        catch (CheckoutException ex)
        {
            return Result<CheckoutDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<CheckoutDto>.Failure($"Failed to create checkout: {ex.Message}");
        }
    }
}
