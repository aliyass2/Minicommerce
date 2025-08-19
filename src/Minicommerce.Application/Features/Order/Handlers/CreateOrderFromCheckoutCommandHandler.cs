using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Orders.Dtos;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Orders;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Orders.Create;

public sealed class CreateOrderFromCheckoutCommandHandler
    : IRequestHandler<CreateOrderFromCheckoutCommand, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderFromCheckoutCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(CreateOrderFromCheckoutCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new OrderException("User must be authenticated to place an order.");

        var checkoutRepo = _uow.Repository<Domain.Checkout.Checkout>();
        var orderRepo = _uow.Repository<Domain.Orders.Order>();
        var cartRepo = _uow.Repository<Domain.Cart.Cart>(); // optional: clear cart after order

        var checkout = await checkoutRepo.FirstOrDefaultAsync(c => c.Id == request.CheckoutId && c.UserId == userId, ct);
        if (checkout is null)
            throw new OrderException("Checkout not found.");

        // Must be completed (Paid -> Completed) before creating order
        var order = Domain.Orders.Order.FromCheckout(checkout);

        await orderRepo.AddAsync(order, ct);

        // Optional: clear user's cart after order (interview-friendly behavior)
        var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        cart?.Clear();

        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<OrderDto>(order);
    }
}
