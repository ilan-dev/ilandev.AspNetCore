namespace ilandev.AspNetCore.reCAPTCHA;

public class RecaptchaOptions
{
    public const string Section = "Recaptcha";

    /// <summary>
    /// Gets the token verification URL.
    /// </summary>
    public string TokenVerifyUrl { get; init; } = "https://www.google.com/recaptcha/api/siteverify";

    /// <summary>
    /// Gets the site secret key, used for verifying tokens.
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// Gets whether a <see cref="RecaptchaVerificationException"/> should be thrown if the token verification fails.
    /// </summary>
    public bool ThrowOnError { get; init; } = false;
}
