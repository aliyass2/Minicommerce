using AutoMapper;
using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Checkout.Complete;

public sealed class CompleteCheckoutCommandHandler
    : IRequestHandler<CompleteCheckoutCommand, CheckoutDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CompleteCheckoutCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CheckoutDto> Handle(CompleteCheckoutCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new CheckoutException("User must be authenticated to complete checkout.");

        var checkoutRepo = _uow.Repository<Domain.Checkout.Checkout>();
        var checkout = await checkoutRepo.FirstOrDefaultAsync(c => c.Id == request.CheckoutId && c.UserId == userId, ct);
        if (checkout is null)
            throw new CheckoutException("Checkout not found.");

        checkout.Complete();

        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<CheckoutDto>(checkout);
    }
}
