using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Swapi.Services.HttpRequest;

public class SwapiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException)
        {
            context.Result = new NotFoundResult();
        }

        if (context.Exception is ProxyRateLimitedException)
        {
            context.Result = new ContentResult
            {
                Content = context.Exception.Message,
                StatusCode = 429
            };
        }
    }
}