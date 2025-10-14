using System.Text.Json;
using Auth.Application.Exceptions;
using Auth.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        switch (exception)
        {
            case DtoException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            
            case UserException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Title = "User Error";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            
            case UnauthorizedException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2";
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                response.StatusCode = StatusCodes.Status401Unauthorized;
                break;
            
            case ConflictException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10";
                problemDetails.Title = "Conflict";
                problemDetails.Detail = exception.Message;
                problemDetails.Status = StatusCodes.Status409Conflict;
                response.StatusCode = StatusCodes.Status409Conflict;
                break;

            default:
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = environment.IsDevelopment() ? exception.Message : "An error occurred while processing your request.";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }
      
        var json = JsonSerializer.Serialize(problemDetails);
        await response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}