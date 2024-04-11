using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ilandev.Extensions.Configuration;

/// <summary>
/// Service collection extensions for <see cref="IAppOptions"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a configuration instance which <typeparamref name="TOptions"/> will bind against, using <typeparamref name="TOptions"/>'s Section property.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration being bound</param>
    /// <returns>The <see cref="IServiceCollection"/> so additional calls can be chained.</returns>
    public static IServiceCollection ConfigureAppOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, IAppOptions => services.Configure<TOptions>(
        configuration.GetSection(TOptions.Section)
    );
}