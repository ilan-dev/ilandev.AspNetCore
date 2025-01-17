﻿using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ilandev.AspNetCore.reCAPTCHA;

/// <summary>
/// A client for verifying reCAPTCHA tokens.
/// </summary>
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

/// <summary>
/// Default implementation for <see cref="IRecaptchaClient"/>.
/// </summary>
public class RecaptchaClient(
    HttpClient httpClient,
    IOptions<RecaptchaOptions> options
) : IRecaptchaClient
{
    /// <inheritdoc />
    public async Task<RecaptchaVerifyResponse> VerifyAsync(string token, string? remoteIp = default, CancellationToken cancellationToken = default)
    {
        var request = new RecaptchaVerificationRequest
        {
            Secret = options.Value.SecretKey,
            Response = token,
            RemoteIp = remoteIp
        };

        using var reqContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["secret"] = options.Value.SecretKey,
            ["response"] = token,
            ["remoteip"] = remoteIp
        });

        using var response = await httpClient.PostAsync(string.Empty, reqContent, cancellationToken);

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