using System;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;
using Minicommerce.Application.Features.Category.Queries;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Category.Handlers;

public sealed class GetCategoryByIdQueryHandler 
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        try
        {
            var categoryRepo = _uow.Repository<Minicommerce.Domain.Catalog.Category>();
            
            var category = await categoryRepo.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
            
            if (category is null)
                return Result<CategoryDto>.Failure("Category not found.");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"An error occurred while retrieving the Category: {ex.Message}");
        }
    }
}