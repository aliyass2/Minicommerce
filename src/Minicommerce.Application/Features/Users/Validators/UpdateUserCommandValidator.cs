using System;
using FluentValidation;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required")
            .MustAsync(UserExists).WithMessage("User not found");

        RuleFor(x => x.UpdateUserDto)
            .NotNull().WithMessage("User data is required");

        RuleFor(x => x.UpdateUserDto.UserName)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 256).WithMessage("Username must be between 3 and 256 characters")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens")
            .When(x => x.UpdateUserDto != null);

        RuleFor(x => new { x.Id, x.UpdateUserDto.UserName })
            .MustAsync(async (data, cancellationToken) => await BeUniqueUserNameForUpdate(data.Id, data.UserName, cancellationToken))
            .WithMessage("Username already exists")
            .When(x => x.UpdateUserDto != null && !string.IsNullOrEmpty(x.UpdateUserDto.UserName));

        RuleFor(x => x.UpdateUserDto.Roles)
            .NotEmpty().WithMessage("At least one role is required")
            .When(x => x.UpdateUserDto != null);

        RuleFor(x => x.UpdateUserDto.Position)
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters")
            .When(x => x.UpdateUserDto != null && !string.IsNullOrEmpty(x.UpdateUserDto.Position));
    }

    private async Task<bool> UserExists(string id, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        return user != null;
    }

    private async Task<bool> BeUniqueUserNameForUpdate(string id, string userName, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.Users.GetByUserNameAsync(userName, cancellationToken);
        return existingUser == null || existingUser.Id == id;
    }

}