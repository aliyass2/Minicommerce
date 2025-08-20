using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            throw new Exception($"User with ID {request.Id} not found");

        var result = await _userManager.DeleteAsync(user);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to delete user: {errors}");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}