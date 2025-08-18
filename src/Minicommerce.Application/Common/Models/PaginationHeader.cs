using System;

namespace Minicommerce.Application.Common.Models;

public class PaginationHeader
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    public PaginationHeader(int pageNumber, int pageSize, int totalCount, int totalPages)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }
}
