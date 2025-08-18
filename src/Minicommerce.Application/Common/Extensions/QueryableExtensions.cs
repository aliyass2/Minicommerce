using System;
using System.Linq.Expressions;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static Task<PaginatedList<T>> PaginatedListAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
        => PaginatedList<T>.CreateAsync(queryable, pageNumber, pageSize);
        
    // You can add more queryable extensions here as needed
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}