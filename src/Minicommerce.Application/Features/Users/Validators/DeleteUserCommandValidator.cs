using System;
using FluentValidation;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Validators;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required")
            .MustAsync(UserExists).WithMessage("User not found")
            .MustAsync(CanBeDeleted).WithMessage("User cannot be deleted due to existing dependencies");
    }

    private async Task<bool> UserExists(string id, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        return user != null;
    }

    private async Task<bool> CanBeDeleted(string id, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null) return false;

        // Add business logic here to check if user can be deleted
        // For example: check if user has active inspections, etc.
        // For now, we'll allow deletion if user is not active
        return !user.IsActive;
    }
}