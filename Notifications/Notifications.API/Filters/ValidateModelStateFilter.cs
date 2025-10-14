using Microsoft.AspNetCore.Mvc.Filters;
using Notifications.Application.Exceptions;

namespace Notifications.API.Filters;

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