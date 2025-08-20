using AutoMapper;
using MediatR;
using Minicommerce.Application.Catalog.Categories.Create;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

public sealed class AddCategoryCommandHandler
    : IRequestHandler<AddCategoryCommand, Result<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddCategoryCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(AddCategoryCommand request, CancellationToken ct)
    {
        try
        {
            // 1) Validate input
            var name = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return Result<CategoryDto>.Failure("Category name is required.");

            // 2) Uniqueness
            var repo = _uow.Repository<Category>();
            var exists = await repo.AnyAsync(c => c.Name == name, ct);
            if (exists)
                return Result<CategoryDto>.Failure("Category already exists.");

            // 3) Create aggregate
            var category = new Category(name);

            // 4) Persist
            await repo.AddAsync(category, ct);
            await _uow.SaveChangesAsync(ct);

            // 5) Map to DTO
            var dto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"An error occurred while creating the category: {ex.Message}");
        }
    }
}
