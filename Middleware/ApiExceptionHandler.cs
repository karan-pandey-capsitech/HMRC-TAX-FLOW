using HMRC_TAX_FLOW.Application.Authentication;
using HMRC_TAX_FLOW.Application.Clients;
using HMRC_TAX_FLOW.Application.SA100;
using HMRC_TAX_FLOW.Application.Users;
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
            ManagedUserNotFoundException => (
                StatusCodes.Status404NotFound,
                "User not found",
                exception.Message),
            InvalidManagedUserRoleException => (
                StatusCodes.Status400BadRequest,
                "Invalid role",
                exception.Message),
            UsernameAlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Username already exists",
                "Choose a different username."),
            Sa100NotFoundException or ClientNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),
            ClientAccessDeniedException => (
                StatusCodes.Status403Forbidden,
                "Access denied",
                exception.Message),
            Sa100ConflictException => (
                StatusCodes.Status409Conflict,
                "SA100 workflow conflict",
                exception.Message),
            Sa100AlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "SA100 return already exists",
                exception.Message),
            InvalidClientAssignmentException => (
                StatusCodes.Status400BadRequest,
                "Invalid client assignment",
                exception.Message),
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
