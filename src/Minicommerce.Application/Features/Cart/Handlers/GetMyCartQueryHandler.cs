using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Cart.Queries;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Cart.Handlers;

public sealed class GetMyCartQueryHandler
    : IRequestHandler<GetMyCartQuery, Result<CartDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetMyCartQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CartDto>> Handle(GetMyCartQuery request, CancellationToken ct)
    {
        try
        {
            var repo = _uow.Repository<Minicommerce.Domain.Cart.Cart>();

            var cart = await repo
                .GetQueryable()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, ct);

            if (cart is null)
                return Result<CartDto>.Failure("Cart not found for this user.");

            var dto = _mapper.Map<CartDto>(cart);
            return Result<CartDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<CartDto>.Failure($"Failed to fetch cart: {ex.Message}");
        }
    }
}
