using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ilandev.AspNetCore.reCAPTCHA;

public interface IRecaptchaClient
{
    /// <summary>
    /// Sends a token verification request to the reCAPTCHA service.
    /// </summary>
    /// <param name="token">The user response token provided by the reCAPTCHA client-side integration on your site.</param>
    /// <param name="remoteIp">The user's IP address.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the request.</param>
    /// <returns></returns>
    Task<RecaptchaVerifyResponse> VerifyAsync(string token, string? remoteIp = default, CancellationToken cancellationToken = default);
}

public class RecaptchaClient(
    HttpClient httpClient,
    IOptions<RecaptchaOptions> options
) : IRecaptchaClient
{
    public async Task<RecaptchaVerifyResponse> VerifyAsync(string token, string? remoteIp = default, CancellationToken cancellationToken = default)
    {
        var request = new RecaptchaVerificationRequest
        {
            Secret = options.Value.SecretKey,
            Response = token,
            RemoteIp = remoteIp
        };

        using var response = await httpClient.PostAsJsonAsync(
            string.Empty,
            request,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var verifyResponse = await response.Content.ReadFromJsonAsync<RecaptchaVerifyResponse>(cancellationToken: cancellationToken);

        if (verifyResponse is null)
        {
            throw new RecaptchaVerificationException("response-null");
        }

        if (!verifyResponse.Success && options.Value.ThrowOnError)
        {
            throw new RecaptchaVerificationException(verifyResponse.ErrorCodes ?? []);
        }

        return verifyResponse;
    }
}