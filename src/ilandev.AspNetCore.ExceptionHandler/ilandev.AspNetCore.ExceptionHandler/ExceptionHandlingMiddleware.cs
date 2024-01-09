using System.Net;
using System.Net.Mime;
using System.Text.Json;
using ilandev.AspNetCore.ExceptionHandler.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ilandev.AspNetCore.ExceptionHandler;

internal class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> log,
    IWebHostEnvironment environment,
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

        if (exceptionType == typeof(TaskCanceledException) && options.IgnoreTaskCancellation) return;

        if (options.LogExceptionDetails)
        {
            log.LogError(ex, "Caught exception in exception handler");
        }
        else
        {
            log.LogDebug("Caught exception of type {exceptionType}", exceptionType.Name);
        }

        var matchingHandler = options.AllowExceptionInheritance
            ? options.Handlers.Find(f => exceptionType.IsAssignableTo(f.ExceptionType))
            : options.Handlers.Find(f => f.ExceptionType == exceptionType);

        if (matchingHandler is null)
        {
            log.LogDebug("No registered exception handler found for exception of type {exceptionType}, writing default response", exceptionType.Name);

            await WriteDefaultResponseAsync(context, ex);

            return;
        }

        log.LogDebug("Found registered exception handler for exception of type {exceptionType}, running", exceptionType.Name);

        if (matchingHandler.Mapping.DynamicInvoke(ex) is not ExceptionMapping mappingResult)
        {
            log.LogWarning("Exception handler returned null result, writing default response");

            await WriteDefaultResponseAsync(context, ex);

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

        await WriteToResponseAsync(context, mappingResult.Response);
    }

    private async Task WriteDefaultResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

        if (!environment.IsDevelopment() && !options.WriteExceptionDetails) return;

        var response = new ExceptionDetailsResponse
        {
            Message = ex.Message,
            StackTrace = ex.StackTrace
        };

        await WriteToResponseAsync(context, response);
    }

    private async Task WriteToResponseAsync(HttpContext context, object response)
    {
        var responseJson = JsonSerializer.Serialize(response, options.JsonSerializerOptions);

        await context.Response.WriteAsync(responseJson, context.RequestAborted);
    }
}