using System;
using MediatR;
using Microsoft.Extensions.Logging;
using Minicommerce.Application.Common.Interfaces;

namespace Minicommerce.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, 
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? string.Empty;
        var userName = _currentUserService.UserName ?? string.Empty;

        _logger.LogInformation("Minicommerce Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        var response = await next();

        _logger.LogInformation("Minicommerce Response: {Name} {@Response}", requestName, response);

        return response;
    }
}