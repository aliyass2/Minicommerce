using System;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Commands;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Category.Handlers;

public class PatchCategoryCommandHandler : IRequestHandler<PatchCategoryCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PatchCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(PatchCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepository = _unitOfWork.Repository<Minicommerce.Domain.Catalog.Category>();
        
        // Get existing category
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
        {
            return Result<bool>.Failure("category not found");
        }
        try
        {
            // Apply partial updates using domain methods that need to be added to Category entity
            if (request.Category.Name != null)
            {
                category.UpdateName(request.Category.Name);
            }

            // Update the entity
            categoryRepository.Update(category);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (CatalogException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}