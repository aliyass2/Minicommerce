using MediatR;
using Minicommerce.Application.Catalog.Categories.Create;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    public CreateCategoryCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new CatalogException("Category name is required.");

        var repo = _uow.Repository<Category>();
        var exists = await repo.AnyAsync(c => c.Name == request.Name, ct);
        if (exists) throw new CatalogException("Category already exists.");

        var category = new Category(request.Name);
        await repo.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
        return category.Id;
    }
}