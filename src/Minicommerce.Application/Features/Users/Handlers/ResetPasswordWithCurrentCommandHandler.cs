using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class ResetPasswordWithCurrentCommandHandler : IRequestHandler<ResetPasswordWithCurrentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordWithCurrentCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<bool> Handle(ResetPasswordWithCurrentCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            throw new Exception($"User with ID {request.UserId} not found");

        if (!user.IsActive)
            throw new Exception("Cannot reset password for inactive user");

        // Verify current password
        var isCurrentPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!isCurrentPasswordCorrect)
        {
            throw new Exception("Current password is incorrect");
        }

        // Change password using ChangePasswordAsync (more secure than remove/add)
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to reset password: {errors}");
        }

        // Update the UpdatedAt timestamp
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}