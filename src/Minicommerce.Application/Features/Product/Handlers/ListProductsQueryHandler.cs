using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Catalog.Products.Models;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Catalog.Products.List;

public sealed class ListProductsQueryHandler
    : IRequestHandler<ListProductsQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ListProductsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ProductDto>>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        try
        {
            var page     = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var repo = _uow.Repository<Product>();

            Expression<Func<Product, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(request.Search)
                    || p.Name.Contains(request.Search!)
                    || (p.Description != null && p.Description.Contains(request.Search!)))
                && (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId.Value);

            Expression<Func<Product, object>> orderBy = request.Sort?.ToLowerInvariant() switch
            {
                "price_asc" or "price_desc" => p => p.Price.Amount,
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
                orderByDescending: orderByDescending,
                includes: [ p => p.Category ] 
            );

            var items = _mapper.Map<List<ProductDto>>(entities);

            var paged = new PaginatedList<ProductDto>(
                items: items,
                count: total,          
                pageNumber: page,
                pageSize: pageSize
            );

            return Result<PaginatedList<ProductDto>>.Success(paged);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<ProductDto>>.Failure($"Failed to list products: {ex.Message}");
        }
    }
}
