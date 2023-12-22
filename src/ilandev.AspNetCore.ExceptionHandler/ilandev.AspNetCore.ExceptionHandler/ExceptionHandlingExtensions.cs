using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ilandev.AspNetCore.ExceptionHandler;

/// <summary>
///     Service collection extensions for the exception handling middleware
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    ///     Adds the exception handling middleware to the service collection.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static IServiceCollection AddExceptionHandlingMiddleware(this IServiceCollection services, Action<ExceptionHandlingOptions>? configureOptions = default)
    {
        services.AddScoped<ExceptionHandlingMiddleware>();

        var opts = new ExceptionHandlingOptions();

        configureOptions?.Invoke(opts);

        services.AddSingleton(opts);

        return services;
    }

    /// <summary>
    ///     Adds the exception handling middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> instance.</param>
    /// <returns>The <see cref="IApplicationBuilder" /> instance.</returns>
    // ReSharper disable once UnusedMember.Global
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}