namespace ilandev.AspNetCore.reCAPTCHA;

public class RecaptchaVerificationException(params string[] errorCodes) : Exception
{
    public string[] ErrorCodes { get; } = errorCodes;
}
