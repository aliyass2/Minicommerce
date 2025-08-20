using System;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Commands;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Category.Handlers;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Minicommerce.Domain.Catalog.Category>();
        
        var entity = await categoryRepo.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            return Result.Failure($"Category with ID {request.Id} was not found");
        }

        categoryRepo.HardDelete(entity);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}