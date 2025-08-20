using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            throw new Exception($"User with ID {request.UserId} not found");

        if (!user.IsActive)
            throw new Exception("Cannot reset password for inactive user");

        // Remove current password and set new one
        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
        {
            var errors = string.Join(", ", removePasswordResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to remove current password: {errors}");
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to set new password: {errors}");
        }

        // Update the UpdatedAt timestamp
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
