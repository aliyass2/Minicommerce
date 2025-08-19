using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Catalog.Products.Models;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Catalog.Products.GetById;

public sealed class GetProductByIdQueryHandler 
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        try
        {
            var productRepo = _uow.Repository<Product>();
            
            var product = await productRepo.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
            
            if (product is null)
                return Result<ProductDto>.Failure("Product not found.");

            var productDto = _mapper.Map<ProductDto>(product);
            return Result<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"An error occurred while retrieving the product: {ex.Message}");
        }
    }
}