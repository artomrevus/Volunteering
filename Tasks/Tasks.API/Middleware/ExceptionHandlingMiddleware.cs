using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tasks.Application.Exceptions;
using Tasks.Domain.Exceptions;

namespace Tasks.API.Middleware;

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
                problemDetails.Title = "Validation Error";
                problemDetails.Status =  StatusCodes.Status400BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Detail = exception.Message;
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            
            case TaskException:
                problemDetails.Title = "Task Error";
                problemDetails.Status =  StatusCodes.Status400BadRequest;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Detail = exception.Message;
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case UnauthorizedException:
                problemDetails.Title = "Unauthorized";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2";
                problemDetails.Detail = exception.Message;
                response.StatusCode = StatusCodes.Status401Unauthorized;
                break;
            
            case ForbiddenException:
                problemDetails.Title = "Forbidden";
                problemDetails.Status = StatusCodes.Status403Forbidden;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4";
                problemDetails.Detail = exception.Message;
                response.StatusCode = StatusCodes.Status403Forbidden;
                break;
            
            case NotFoundException:
                problemDetails.Title = "Not Found";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
                problemDetails.Detail = exception.Message;
                response.StatusCode = StatusCodes.Status404NotFound;
                break;

            default:
                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
                problemDetails.Detail = environment.IsDevelopment() ? exception.Message : "An error occurred while processing your request.";
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