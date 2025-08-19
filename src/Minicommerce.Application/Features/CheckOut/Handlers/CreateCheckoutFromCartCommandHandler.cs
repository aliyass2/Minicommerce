using AutoMapper;
using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Checkout.Create;

public sealed class CreateCheckoutFromCartCommandHandler
    : IRequestHandler<CreateCheckoutFromCartCommand, CheckoutDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateCheckoutFromCartCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CheckoutDto> Handle(CreateCheckoutFromCartCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new CheckoutException("User must be authenticated to start checkout.");

        var cartRepo = _uow.Repository<Domain.Cart.Cart>();
        var checkoutRepo = _uow.Repository<Domain.Checkout.Checkout>();

        var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (cart is null || !cart.Items.Any())
            throw new CheckoutException("Cannot create checkout from empty or missing cart.");

        var checkout = Domain.Checkout.Checkout.FromCart(cart);

        await checkoutRepo.AddAsync(checkout, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<CheckoutDto>(checkout);
    }
}
