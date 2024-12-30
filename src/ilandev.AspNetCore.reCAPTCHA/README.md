# ilandev.AspNetCore.reCAPTCHA

[![nuget.org](https://img.shields.io/nuget/dt/ilandev.AspNetCore.reCAPTCHA?style=flat-square&logo=nuget)](https://www.nuget.org/packages/ilandev.AspNetCore.reCAPTCHA)

A simple reCAPTCHA v3 verification client for ASP.NET Core.

## Usage

1. Call `builder.Services.AddRecaptcha()` and either pass in a configuration action to configure the middleware, or `builder.Configuration` to read the default configuration at the `Recaptcha` configuration section.

## Examples

```csharp
// Reads from the "Recaptcha" configuration section

builder.Services.AddRecaptcha(builder.Configuration);

// or..

builder.Services.AddRecaptcha(opts => 
{
	opts.SecretKey = "your secret key";
});

// in your controller..

public class MyController(IRecaptchaClient recaptchaClient) : ControllerBase
{
	[HttpPost("register")]
	public async Task Register([FromBody] RegisterModel model, CancellationToken cancellationToken)
	{
		var recaptchaResponse = await recaptchaClient.VerifyAsync(
			model.Token,
			HttpContext.Connection.RemoteIpAddress, // optional
			cancellationToken
		);
		
		if (!recaptchaResponse.Success)
		{
			// handle the error
		}
		
		// continue with registration
	}
}
```

## Middleware Configuration

| Configuration | Default Value | Details |
| - | - | - |
| TokenVerifyUrl | https://www.google.com/recaptcha/api/siteverify | The reCAPTCHA v3 verification endpoint. |
| SecretKey | | Your reCAPTCHA v3 secret key, found in the Google Cloud Console. |
| ThrowOnError | false | If true, will throw a `RecaptchaVerificationException` when token verification fails. |
