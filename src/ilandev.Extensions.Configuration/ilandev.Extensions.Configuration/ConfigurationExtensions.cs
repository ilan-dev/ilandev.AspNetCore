using Microsoft.Extensions.Configuration;

namespace ilandev.Extensions.Configuration;

/// <summary>
/// <see cref="IConfiguration"/> extensions for <see cref="IAppOptions"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Attempts to bind the configuration instance to a new instance of type <typeparamref name="TOptions"/>, at the configuration section defined by the <typeparamref name="TOptions"/>.Section property.
    /// </summary>
    /// <typeparam name="TOptions">The type of the new instance to bind.</typeparam>
    /// <param name="configuration">The configuration instance to bind.</param>
    /// <returns>The new instance of <typeparamref name="TOptions"/> if successful, default(<typeparamref name="TOptions"/>) otherwise.</returns>
    public static TOptions? GetAppOptions<TOptions>(this IConfiguration configuration)
        where TOptions : class, IAppOptions => configuration.GetSection(TOptions.Section)
                                                            .Get<TOptions>();

    /// <summary>
    /// Attempts to bind the configuration instance to a new instance of type <typeparamref name="TOptions"/>, at the configuration section defined by the <typeparamref name="TOptions"/>.Section property. If unsuccessful, an <see cref="ArgumentException"/> is thrown.
    /// </summary>
    /// <typeparam name="TOptions">The type of the new instance to bind.</typeparam>
    /// <param name="configuration">The configuration instance to bind.</param>
    /// <exception cref="ArgumentException">Thrown if unable to bind the new instance of type <typeparamref name="TOptions"/>.</exception>
    /// <returns>The new instance of <typeparamref name="TOptions"/>.</returns>
    public static TOptions GetRequiredAppOptions<TOptions>(this IConfiguration configuration)
        where TOptions : class, IAppOptions => configuration.GetAppOptions<TOptions>()
                                               ?? throw new ArgumentException($"Could not bind {typeof(TOptions).Name} from configuration", nameof(configuration));
}