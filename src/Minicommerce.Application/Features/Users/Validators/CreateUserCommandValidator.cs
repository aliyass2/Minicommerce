using System;
using FluentValidation;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.CreateUserDto)
            .NotNull().WithMessage("User data is required");

        RuleFor(x => x.CreateUserDto.UserName)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 256).WithMessage("Username must be between 3 and 256 characters")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens")
            .MustAsync(BeUniqueUserName).WithMessage("Username already exists")
            .When(x => x.CreateUserDto != null);

        RuleFor(x => x.CreateUserDto.Roles)
            .NotEmpty().WithMessage("At least one role is required")
            .When(x => x.CreateUserDto != null);

        RuleFor(x => x.CreateUserDto.Position)
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters")
            .When(x => x.CreateUserDto != null && !string.IsNullOrEmpty(x.CreateUserDto.Position));

        RuleFor(x => x.CreateUserDto.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one digit")
            .When(x => x.CreateUserDto != null);
    }

    private async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.Users.GetByUserNameAsync(userName, cancellationToken);
        return existingUser == null;
    }

}