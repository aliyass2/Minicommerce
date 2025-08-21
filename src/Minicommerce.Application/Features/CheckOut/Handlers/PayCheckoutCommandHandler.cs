using AutoMapper;
using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Checkout.Pay;

public sealed class PayCheckoutCommandHandler
    : IRequestHandler<PayCheckoutCommand, Result<CheckoutDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public PayCheckoutCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<CheckoutDto>> Handle(PayCheckoutCommand request, CancellationToken ct)
    {
        try
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                throw new CheckoutException("User must be authenticated to pay checkout.");

            var checkoutRepo = _uow.Repository<Domain.Checkout.Checkout>();
            var checkout = await checkoutRepo.FirstOrDefaultAsync(
                c => c.Id == request.CheckoutId && c.UserId == userId, ct);

            if (checkout is null)
                throw new CheckoutException("Checkout not found.");

            var txnId = string.IsNullOrWhiteSpace(request.TransactionId)
                ? Guid.NewGuid().ToString()
                : request.TransactionId;

            checkout.MakePayment(new PaymentInfo(request.PaymentMethod, txnId, isMocked: true));

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
            return Result<CheckoutDto>.Failure($"Failed to pay checkout: {ex.Message}");
        }
    }
}
