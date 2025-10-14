using Microsoft.AspNetCore.Mvc.Filters;
using Tasks.Application.Exceptions;

namespace Tasks.API.Filters;

public class ValidateModelStateFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errorMessages = context.ModelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            
            throw new DtoException(string.Join(", ", errorMessages));
        }

        await next();
    }
}