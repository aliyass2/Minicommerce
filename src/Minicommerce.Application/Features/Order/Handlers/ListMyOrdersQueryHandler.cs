using System;
using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Order.Queries;
using Minicommerce.Application.Orders.Dtos;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Order.Handlers;

public sealed class ListMyOrdersQueryHandler
    : IRequestHandler<ListMyOrdersQuery, Result<PaginatedList<OrderDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ListMyOrdersQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<OrderDto>>> Handle(ListMyOrdersQuery request, CancellationToken ct)
    {
        try
        {
            var page     = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var repo = _uow.Repository<Minicommerce.Domain.Orders.Order>();

            Expression<Func<Minicommerce.Domain.Orders.Order, bool>> predicate = o => o.UserId == request.UserId;

            var sort = request.Sort?.ToLowerInvariant();
            Expression<Func<Minicommerce.Domain.Orders.Order, object>> orderBy = sort switch
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

            var items = _mapper.Map<List<OrderDto>>(entities);
            var paged = new PaginatedList<OrderDto>(items, total, page, pageSize);

            return Result<PaginatedList<OrderDto>>.Success(paged);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<OrderDto>>.Failure($"Failed to list your orders: {ex.Message}");
        }
    }
}
