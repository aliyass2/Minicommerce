using System;
using MediatR;
using Minicommerce.Application.Catalog.Products.Delete;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Catalog.Products.Delete;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productRepo = _unitOfWork.Repository<Product>();
        
        var entity = await productRepo.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            return Result.Failure($"Product with ID {request.Id} was not found");
        }



        productRepo.HardDelete(entity);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}