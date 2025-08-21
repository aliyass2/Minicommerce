using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Features.Users.Commands;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Application.Features.Users.Queries;

namespace Minicommerce.WebApi.Controllers
{

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetAllUsers(
        [FromQuery] bool? isActive = null,
        [FromQuery] string? role = null
        )
    {
        try
        {
            var query = new GetAllUsersQuery
            {
                IsActive = isActive,
                Role = role,
            };

            var users = await _mediator.Send(query);
            return Ok(users);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        try
        {
            var query = new GetUserByIdQuery { Id = id };
            var user = await _mediator.Send(query);

            if (user == null)
                return NotFound(new { message = $"User with ID '{id}' not found" });

            return Ok(user);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
        }
    }
    [HttpGet("roles")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        try
        {
            var query = new GetAllRolesQuery();
            var roles = await _mediator.Send(query);
            return Ok(roles);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving roles", error = ex.Message });
        }
    }

    [HttpGet("by-role/{role}")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsersByRole(string role)
    {
        try
        {
            var query = new GetUsersByRoleQuery { Role = role };
            var users = await _mediator.Send(query);
            return Ok(users);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var command = new CreateUserCommand { CreateUserDto = createUserDto };
            var user = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the user", error = ex.Message });
        }
    }
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var command = new UpdateUserCommand { Id = id, UpdateUserDto = updateUserDto };
            var user = await _mediator.Send(command);
            return Ok(user);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = "Validation failed", errors = ex.Errors });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
        }
    }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ForceDeleteUser(string id)
        {
            // prevent deleting the currently logged-in user
            var currentUserId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("nameid")?.Value ??
                User.FindFirst("sub")?.Value;

            if (string.Equals(currentUserId, id, StringComparison.Ordinal))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Cannot delete your own account"
                });
            }

            try
            {
                var succeeded = await _mediator.Send(new DeleteUserCommand { Id = id });

                if (succeeded)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "User deleted successfully"
                    });
                }

                // Handler currently throws on failures, but keep a safe fallback
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to delete user"
                });
            }
            catch (Exception ex)
            {
                // The handler throws generic Exception with “not found” or identity errors.
                var isNotFound = ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase);

                if (isNotFound)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while force deleting the user",
                    error = ex.Message
                });
            }
        }
[HttpPost("{id}/reset-password")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult> ResetPassword(string id, [FromBody] ResetPasswordDto resetPasswordDto)
{
    try
    {
        var command = new ResetPasswordCommand 
        { 
            UserId = id, 
            NewPassword = resetPasswordDto.NewPassword 
        };
        
        var result = await _mediator.Send(command);
        
        if (result)
        {
            return Ok(new { message = "Password reset successfully" });
        }
        
        return BadRequest(new { message = "Failed to reset password" });
    }
    catch (ValidationException ex)
    {
        return BadRequest(new { message = "Validation failed", errors = ex.Errors });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "An error occurred while resetting password", error = ex.Message });
    }
}

[HttpPost("reset-my-password")]
[Authorize] // Any authenticated user
public async Task<ActionResult> ResetMyPassword([FromBody] ResetPasswordWithCurrentDto resetPasswordDto)
{
    try
    {
        // Get current user ID from JWT token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not found in token" });

        var command = new ResetPasswordWithCurrentCommand 
        { 
            UserId = userId, 
            CurrentPassword = resetPasswordDto.CurrentPassword,
            NewPassword = resetPasswordDto.NewPassword 
        };
        
        var result = await _mediator.Send(command);
        
        if (result)
        {
            return Ok(new { message = "Password reset successfully" });
        }
        
        return BadRequest(new { message = "Failed to reset password" });
    }
    catch (ValidationException ex)
    {
        return BadRequest(new { message = "Validation failed", errors = ex.Errors });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "An error occurred while resetting password", error = ex.Message });
    }
}

}
}
