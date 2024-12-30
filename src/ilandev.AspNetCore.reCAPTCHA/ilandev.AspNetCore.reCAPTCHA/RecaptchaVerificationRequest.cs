using System.Text.Json.Serialization;

namespace ilandev.AspNetCore.reCAPTCHA;

internal class RecaptchaVerificationRequest
{
    [JsonPropertyName("secret")]
    public required string Secret { get; set; }

    [JsonPropertyName("response")]
    public required string Response { get; set; }

    [JsonPropertyName("remoteip")]
    public string? RemoteIp { get; set; }
}