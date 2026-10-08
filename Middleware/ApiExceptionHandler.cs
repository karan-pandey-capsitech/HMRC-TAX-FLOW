using HMRC_TAX_FLOW.Application.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Middleware;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            logger.LogError(
                exception,
                "Unhandled exception after the response started for {RequestMethod} {RequestPath}",
                httpContext.Request.Method,
                httpContext.Request.Path);
            return false;
        }

        var (statusCode, title, detail) = exception switch
        {
            InvalidCredentialsException => (
                StatusCodes.Status401Unauthorized,
                "Authentication failed",
                "The username or password is incorrect."),
            UsernameAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Username already exists",
                "Choose a different username."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "The request could not be completed.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception processing {RequestMethod} {RequestPath}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            logger.LogInformation(
                "Request {RequestMethod} {RequestPath} failed with {StatusCode}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                statusCode);
        }

        httpContext.Response.StatusCode = statusCode;
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            Exception = exception
        });

        return true;
    }
}
