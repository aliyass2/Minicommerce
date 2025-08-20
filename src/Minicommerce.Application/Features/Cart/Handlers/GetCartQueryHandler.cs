using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Cart.Dtos;
using Minicommerce.Application.Features.Cart.Queries;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Cart.Handlers;

public sealed class GetCartQueryHandler
    : IRequestHandler<GetCartQuery, Result<PaginatedList<CartDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCartQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<CartDto>>> Handle(GetCartQuery request, CancellationToken ct)
    {
        try
        {
            var page     = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var repo = _uow.Repository<Minicommerce.Domain.Cart.Cart>();

            // No search predicate (GetCartQuery has no Search)
        Expression<Func<Minicommerce.Domain.Cart.Cart, bool>> predicate =
            string.IsNullOrWhiteSpace(request.UserId) 
                ? _ => true 
                : c => c.UserId == request.UserId;

            // Sorting: support "total_asc" / "total_desc"; default to CreatedAt desc (if present), else TotalPrice desc
            var sort = request.Sort?.ToLowerInvariant();

            Expression<Func<Minicommerce.Domain.Cart.Cart, object>> orderBy =
                sort switch
                {
                    "total_asc" or "total_desc" => c => c.TotalPrice,
                    _ => (Expression<Func<Minicommerce.Domain.Cart.Cart, object>>)(c => c.CreatedAt) // change to c => c.TotalPrice if you don't have CreatedAt
                };

            bool orderByDescending =
                sort switch
                {
                    "total_asc"  => false,
                    "total_desc" => true,
                    _            => true
                };

            var (entities, total) = await repo.GetPagedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                orderByDescending: orderByDescending,
                // include items so CartDto.Items maps
                includes: c => c.Items
            );

            var items = _mapper.Map<List<CartDto>>(entities);

            var paged = new PaginatedList<CartDto>(
                items: items,
                count: total,
                pageNumber: page,
                pageSize: pageSize
            );

            return Result<PaginatedList<CartDto>>.Success(paged);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<CartDto>>.Failure($"Failed to list carts: {ex.Message}");
        }
    }
}
