using AutoMapper;
using MediatR;
using Minicommerce.Application.Catalog.Products.Models;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Catalog.Products.Add;

public sealed class AddProductCommandHandler
    : IRequestHandler<AddProductCommand, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddProductCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(AddProductCommand request, CancellationToken ct)
    {
        var categoryRepo = _uow.Repository<Category>();
        var productRepo  = _uow.Repository<Product>();

        // 1) Ensure category exists
        var category = await categoryRepo.FirstOrDefaultAsync(c => c.Id == request.CategoryId, ct);
        if (category is null)
            throw new CatalogException("Category not found.");

        // 2) Optional uniqueness check by name within category
        var nameExists = await productRepo.AnyAsync(
            p => p.Name == request.Name && p.CategoryId == request.CategoryId, ct);
        if (nameExists)
            throw new CatalogException("A product with the same name already exists in this category.");

        // 3) Create aggregate
        var money = new Money(
            amount: request.Price,
            currency: string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency);

        var product = new Product(
            name: request.Name,
            description: request.Description ?? string.Empty,
            price: money,
            stockQuantity: request.StockQuantity,
            category: category
        );

        // 4) Persist
        await productRepo.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        // 5) Map to DTO (category is already set on the entity we created)
        return _mapper.Map<ProductDto>(product);
    }
}
