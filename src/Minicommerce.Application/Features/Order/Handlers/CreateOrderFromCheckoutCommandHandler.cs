using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Orders.Dtos;
using Minicommerce.Domain.Orders;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Orders.Create;

public sealed class CreateOrderFromCheckoutCommandHandler
    : IRequestHandler<CreateOrderFromCheckoutCommand, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderFromCheckoutCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

public async Task<OrderDto> Handle(CreateOrderFromCheckoutCommand request, CancellationToken ct)
{
    var userId = _currentUser.UserId ?? throw new OrderException("User must be authenticated to place an order.");

    var checkoutRepo = _uow.Repository<Domain.Checkout.Checkout>();
    var orderRepo    = _uow.Repository<Domain.Orders.Order>();
    var productRepo  = _uow.Repository<Domain.Catalog.Product>();
    var cartRepo     = _uow.Repository<Domain.Cart.Cart>();

    var checkout = await checkoutRepo
        .GetQueryable()
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == request.CheckoutId && c.UserId == userId, ct);

    if (checkout is null) throw new OrderException("Checkout not found.");
    if (checkout.Items is null || checkout.Items.Count == 0) throw new OrderException("Checkout has no items.");

    // ↓↓↓ Decrement stock here (atomic with order save)
    var productIds = checkout.Items.Select(i => i.ProductId).ToList();
    var products = await productRepo
        .GetQueryable()
        .Where(p => productIds.Contains(p.Id))
        .ToDictionaryAsync(p => p.Id, ct);

    foreach (var item in checkout.Items)
    {
        if (!products.TryGetValue(item.ProductId, out var prod))
            throw new OrderException($"Product not found: {item.ProductId}");

        // Will throw if not enough stock
        prod.DecreaseStock(item.Quantity);
    }

    var order = Domain.Orders.Order.FromCheckout(checkout);
    await orderRepo.AddAsync(order, ct);

    // optional: clear cart
    var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
    cart?.Clear();

    await _uow.SaveChangesAsync(ct);  // stock + order saved together

    return _mapper.Map<OrderDto>(order);
}

}
