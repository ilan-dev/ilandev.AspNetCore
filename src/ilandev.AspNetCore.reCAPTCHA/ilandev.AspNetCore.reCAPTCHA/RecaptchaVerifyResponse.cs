using System.Text.Json.Serialization;

namespace ilandev.AspNetCore.reCAPTCHA;

/// <summary>
/// Represents the response from the reCAPTCHA verification endpoint.
/// </summary>
public class RecaptchaVerifyResponse
{
    /// <summary>
    /// Gets whether the token was successfully verified.
    /// </summary>
    [JsonPropertyName("success")]
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the timestamp of the challenge load.
    /// </summary>
    [JsonPropertyName("challenge_ts")]
    public DateTime? ChallengeTimestamp { get; init; }

    /// <summary>
    /// Gets the hostname of the site where the reCAPTCHA was solved.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }

    /// <summary>
    /// Gets a list of error codes returned from the reCAPTCHA verification endpoint.
    /// </summary>
    [JsonPropertyName("error-codes")]
    public string[]? ErrorCodes { get; init; }
}