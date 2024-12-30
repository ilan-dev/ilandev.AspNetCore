namespace ilandev.AspNetCore.reCAPTCHA;

/// <summary>
/// Exception thrown when a reCAPTCHA token verification fails.
/// </summary>
/// <param name="errorCodes">A list of error codes returned from the reCAPTCHA verification endpoint.</param>
public class RecaptchaVerificationException(params string[] errorCodes) : Exception
{
    /// <summary>
    /// List of error codes returned from the reCAPTCHA verification endpoint.
    /// </summary>
    public string[] ErrorCodes { get; } = errorCodes;
}
