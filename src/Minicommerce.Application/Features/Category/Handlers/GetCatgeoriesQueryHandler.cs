using System;
using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;
using Minicommerce.Application.Features.Category.Queries;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Category.Handlers;

public sealed class GetCatgeoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<PaginatedList<CategoryDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCatgeoriesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        try
        {
            var page     = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var repo = _uow.Repository<Minicommerce.Domain.Catalog.Category>();

            Expression<Func<Minicommerce.Domain.Catalog.Category, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(request.Search)
                    || p.Name.Contains(request.Search!));

            Expression<Func<Minicommerce.Domain.Catalog.Category, object>> orderBy = request.Sort?.ToLowerInvariant() switch
            {
                "name_desc" or "name" or _  => p => p.Name
            };

            bool orderByDescending = request.Sort?.ToLowerInvariant() switch
            {
                "price_desc" => true,
                "name_desc"  => true,
                _            => false
            };

            var (entities, total) = await repo.GetPagedAsync(
                pageNumber: page,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: orderBy,
                orderByDescending: orderByDescending
            );

            var items = _mapper.Map<List<CategoryDto>>(entities);

            var paged = new PaginatedList<CategoryDto>(
                items: items,
                count: total,          
                pageNumber: page,
                pageSize: pageSize
            );

            return Result<PaginatedList<CategoryDto>>.Success(paged);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<CategoryDto>>.Failure($"Failed to list products: {ex.Message}");
        }
    }
}
