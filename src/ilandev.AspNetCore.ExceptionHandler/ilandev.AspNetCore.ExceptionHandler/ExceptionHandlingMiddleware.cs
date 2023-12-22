using System.Net.Mime;
using System.Text.Json;
using ilandev.AspNetCore.ExceptionHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ilandev.AspNetCore.ExceptionHandler;

internal class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> log,
    ExceptionHandlingOptions options
) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                log.LogDebug("Caught an exception, but response has already started. Skipping handling of exception..");

                return;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var exceptionType = ex.GetType();

        if (exceptionType == typeof(TaskCanceledException) && options.IgnoreTaskCancellation)
        {
            return;
        }

        log.LogError("Caught exception of type {exceptionType}", exceptionType.Name);

        var matchingHandler = options.AllowExceptionInheritance
            ? options.Handlers.Find(f => exceptionType.IsAssignableTo(f.ExceptionType))
            : options.Handlers.Find(f => f.ExceptionType == exceptionType);

        if (matchingHandler is null)
        {
            log.LogDebug("No registered exception handler found for exception of type {exceptionType}, skipping", exceptionType.Name);

            return;
        }

        log.LogDebug("Found registered exception handler for exception of type {exceptionType}, running", exceptionType.Name);

        ExceptionMapping? mappingResult = matchingHandler.Mapping.DynamicInvoke(exceptionType) as ExceptionMapping;

        if (mappingResult is null)
        {
            log.LogWarning("Exception handler returned null result, skipping");

            return;
        }

        context.Response.StatusCode = (int) mappingResult.StatusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        if (mappingResult.Response is null)
        {
            log.LogTrace("Exception handler returned null response, skipping writing of response");

            return;
        }

        context.RequestAborted.ThrowIfCancellationRequested();

        var responseJson = JsonSerializer.Serialize(mappingResult.Response, options.JsonSerializerOptions);

        await context.Response.WriteAsync(responseJson, context.RequestAborted);
    }
}