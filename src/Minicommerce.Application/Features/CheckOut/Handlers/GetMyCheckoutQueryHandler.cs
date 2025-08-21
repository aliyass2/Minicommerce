using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.CheckOut.Queries;
using Minicommerce.Domain.Repositories;
using Minicommerce.Domain.Checkout;

namespace Minicommerce.Application.Features.CheckOut.Handlers;

public sealed class GetMyCheckoutQueryHandler
    : IRequestHandler<GetMyCheckoutQuery, Result<PaginatedList<CheckoutDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetMyCheckoutQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<CheckoutDto>>> Handle(GetMyCheckoutQuery request, CancellationToken ct)
    {
        try
        {
            var page     = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var repo = _uow.Repository<Minicommerce.Domain.Checkout.Checkout>();

            // Updated predicate to filter out completed checkouts
            Expression<Func<Minicommerce.Domain.Checkout.Checkout, bool>> predicate = o => 
                o.UserId == request.UserId && 
                o.Status != CheckoutStatus.Completed;

            var sort = request.Sort?.ToLowerInvariant();
            Expression<Func<Minicommerce.Domain.Checkout.Checkout, object>> orderBy = sort switch
            {
                "total_asc" or "total_desc" => o => o.TotalAmount,
                "status" or "status_desc"   => o => o.Status,
                _                           => o => o.CreatedAt
            };
            bool orderByDesc = sort switch
            {
                "total_asc"    => false,
                "status"       => false,
                "status_desc"  => true,
                "total_desc"   => true,
                _              => true
            };

            var (entities, total) = await repo.GetPagedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                orderByDescending: orderByDesc,
                includes: o => o.Items
            );

            var items = _mapper.Map<List<CheckoutDto>>(entities);
            var paged = new PaginatedList<CheckoutDto>(items, total, page, pageSize);

            return Result<PaginatedList<CheckoutDto>>.Success(paged);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<CheckoutDto>>.Failure($"Failed to list your Checkout: {ex.Message}");
        }
    }
}