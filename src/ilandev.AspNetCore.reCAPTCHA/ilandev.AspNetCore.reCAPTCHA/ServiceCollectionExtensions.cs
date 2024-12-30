using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ilandev.AspNetCore.reCAPTCHA;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures and adds the <see cref="IRecaptchaClient"/> to the service collection, using the default configuration section.
    /// </summary>
    /// <param name="services">The service collection to add the <see cref="IRecaptchaClient"/> to.</param>
    /// <param name="configuration">The host <see cref="IConfiguration"/> to read the <see cref="RecaptchaOptions"/> from.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddRecaptcha(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddRecaptcha(opts => configuration
                .GetRequiredSection(RecaptchaOptions.Section)
                .Bind(opts)
            );

    /// <summary>
    /// Configures and adds the <see cref="IRecaptchaClient"/> to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the <see cref="IRecaptchaClient"/> to.</param>
    /// <param name="configureOptions">A lambda configuring the <see cref="RecaptchaOptions"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddRecaptcha(
        this IServiceCollection services,
        Action<RecaptchaOptions> configureOptions
    ) => services
        .Configure(configureOptions)
        .AddHttpClient<IRecaptchaClient, RecaptchaClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<RecaptchaOptions>>().Value;

            client.BaseAddress = new Uri(opts.TokenVerifyUrl);
        });
}