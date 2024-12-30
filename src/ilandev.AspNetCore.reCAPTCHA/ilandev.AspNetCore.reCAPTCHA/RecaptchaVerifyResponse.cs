using System.Text.Json.Serialization;

namespace ilandev.AspNetCore.reCAPTCHA;

public class RecaptchaVerifyResponse
{
    [JsonPropertyName("success")]
    public required bool Success { get; init; }

    [JsonPropertyName("challenge_ts")]
    public DateTime? ChallengeTimestamp { get; init; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }

    [JsonPropertyName("error-codes")]
    public string[]? ErrorCodes { get; init; }
}