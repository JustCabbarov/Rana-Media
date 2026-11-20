using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using RanaMedia.Application.Exceptions;
using RanaMedia.Contract.DTOs.Common;

namespace RanaMedia.Presentation.ExceptionHandler;

public class NullExceptionHandler : IExceptionHandler
{
    private readonly ILogger<NullExceptionHandler> _logger;
    public NullExceptionHandler(ILogger<NullExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotNullExceptions ex) return false;
        _logger.LogWarning(ex, "NotNullExceptions: {Message}", ex.Message);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(ex.Message, "NULL_ARGUMENT", traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class BusinessRuleExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BusinessRuleExceptionHandler> _logger;
    public BusinessRuleExceptionHandler(ILogger<BusinessRuleExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BusinessRuleException ex) return false;
        _logger.LogWarning(ex, "BusinessRuleException ({Code}): {Message}", ex.ErrorCode, ex.Message);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(ex.Message, ex.ErrorCode, traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class UnauthorizedExceptionHandler : IExceptionHandler
{
    private readonly ILogger<UnauthorizedExceptionHandler> _logger;
    public UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedException && exception is not UnauthorizedAccessException) return false;
        _logger.LogWarning(exception, "Unauthorized: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(exception.Message, "UNAUTHORIZED", traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class InvalidCredentialsExceptionHandler : IExceptionHandler
{
    private readonly ILogger<InvalidCredentialsExceptionHandler> _logger;
    public InvalidCredentialsExceptionHandler(ILogger<InvalidCredentialsExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not InvalidCredentialsException ex) return false;
        _logger.LogWarning(ex, "InvalidCredentialsException: {Message}", ex.Message);

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(ex.Message, "INVALID_CREDENTIALS", traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class ForbiddenExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ForbiddenExceptionHandler> _logger;
    public ForbiddenExceptionHandler(ILogger<ForbiddenExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ForbiddenException ex) return false;
        _logger.LogWarning(ex, "ForbiddenException ({Code}): {Message}", ex.ErrorCode, ex.Message);

        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(ex.Message, ex.ErrorCode, traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class NotFoundExceptionHandler : IExceptionHandler
{
    private readonly ILogger<NotFoundExceptionHandler> _logger;
    public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException && exception is not KeyNotFoundException) return false;
        _logger.LogWarning(exception, "NotFound: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(exception.Message, "NOT_FOUND", traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class ConflictExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ConflictExceptionHandler> _logger;
    public ConflictExceptionHandler(ILogger<ConflictExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ConflictException ex) return false;
        _logger.LogWarning(ex, "ConflictException ({Code}): {Message}", ex.ErrorCode, ex.Message);

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(ex.Message, ex.ErrorCode, traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var message = _env.IsDevelopment()
            ? exception.Message
            : "Serverdə gözlənilməz xəta baş verdi.";

        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(message, "INTERNAL_SERVER_ERROR", traceId: Activity.Current?.Id ?? httpContext.TraceIdentifier),
            cancellationToken);
        return true;
    }
}

