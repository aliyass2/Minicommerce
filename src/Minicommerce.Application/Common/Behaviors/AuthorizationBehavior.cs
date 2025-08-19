// // Solution 1: Updated AuthorizationBehavior - Only apply to MediatR commands
// using MediatR;
// using Minicommerce.Application.Common.Interfaces;
// using Microsoft.Extensions.Logging;

// namespace Minicommerce.Application.Common.Behaviors;

// public class AuthorizationBehavior<TRequest, TResponse> 
//     : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
// {
//     private readonly ICurrentUserService _currentUserService;
//     private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

//     // List of command types that don't require authorization
//     private static readonly HashSet<string> AnonymousCommands = new()
//     {
//         "LoginCommand",
//         "ForgotPasswordCommand", // If you add password reset
//         "ResetPasswordCommand"
//     };

//     public AuthorizationBehavior(ICurrentUserService currentUserService, ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
//     {
//         _currentUserService = currentUserService;
//         _logger = logger;
//     }

//     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//     {
//         var requestTypeName = typeof(TRequest).Name;
        
//         _logger.LogDebug("Processing authorization for request: {RequestType}", requestTypeName);

//         // Skip authorization for anonymous commands
//         if (AnonymousCommands.Contains(requestTypeName))
//         {
//             _logger.LogDebug("Skipping authorization for anonymous command: {RequestType}", requestTypeName);
//             return await next();
//         }

//         // For all other MediatR requests, ensure user is authenticated
//         if (string.IsNullOrEmpty(_currentUserService.UserId))
//         {
//             _logger.LogWarning("Unauthorized access attempt for request: {RequestType}", requestTypeName);
//             throw new UnauthorizedAccessException($"Authentication required for {requestTypeName}");
//         }

//         _logger.LogDebug("Authorization passed for user: {UserId}, request: {RequestType}", _currentUserService.UserId, requestTypeName);
//         return await next();
//     }
// }
