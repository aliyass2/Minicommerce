using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Catalog.Products.Commands;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;
using Minicommerce.Domain.Common;

namespace Minicommerce.Application.Catalog.Products.Commands;

public class PatchProductCommandHandler : IRequestHandler<PatchProductCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PatchProductCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(PatchProductCommand request, CancellationToken cancellationToken)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        
        // Get existing product
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            return Result<bool>.Failure("Product not found");
        }

        // Verify Category exists if provided
        Category? category = null;
        if (request.Product.CategoryId.HasValue)
        {
            var categoryRepository = _unitOfWork.Repository<Category>();
            category = await categoryRepository.GetByIdAsync(request.Product.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result<bool>.Failure("Category not found");
            }
        }

        try
        {
            // Apply partial updates using domain methods that need to be added to Product entity
            if (request.Product.Name != null)
            {
                product.UpdateName(request.Product.Name);
            }

            if (request.Product.Description != null)
            {
                product.UpdateDescription(request.Product.Description);
            }

            if (request.Product.Price.HasValue && request.Product.Currency != null)
            {
                var newPrice = new Money(request.Product.Price.Value, request.Product.Currency);
                product.UpdatePrice(newPrice);
            }
            else if (request.Product.Price.HasValue)
            {
                // Keep existing currency, update amount only
                var newPrice = new Money(request.Product.Price.Value, product.Price.Currency);
                product.UpdatePrice(newPrice);
            }

            if (request.Product.StockQuantity.HasValue)
            {
                product.UpdateStockQuantity(request.Product.StockQuantity.Value);
            }

            if (request.Product.CategoryId.HasValue && category != null)
            {
                product.UpdateCategory(category);
            }

            // Update the entity
            productRepository.Update(product);
            
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