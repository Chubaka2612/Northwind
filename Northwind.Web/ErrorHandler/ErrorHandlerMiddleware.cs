using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred.");

        context.Response.ContentType = "text/html";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorModel = new
        {
            ErrorMessage = exception.Message,
            StackTrace = exception.StackTrace
        };

        var result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                {"ErrorMessage", errorModel.ErrorMessage},
                {"StackTrace", errorModel.StackTrace}
            }
        };

        await result.ExecuteResultAsync(new ActionContext(context, new RouteData(), new ActionDescriptor()));
    }
}
