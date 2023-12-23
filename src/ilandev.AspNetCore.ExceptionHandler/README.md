# ilandev.AspNetCore.ExceptionHandler

![nuget.org](https://img.shields.io/nuget/dt/ilandev.AspNetCore.ExceptionHandler?style=flat-square&logo=nuget)

Customizable exception mapping middleware for ASP.NET Core. Allows you to specify custom response lambdas for specific exception types.

## Usage

1. Call `builder.Services.AddExceptionHandlingMiddleware()` and pass in a configuration action to configure the middleware.
2. Call `app.UseExceptionHandlingMiddleware()` after the call to `app.UseRouting()`.

## Examples

```csharp
builder.Services.AddExceptionHandlingMiddleware(c => c
	.AddHandler<UserNotFoundException>(e => new ExceptionMapping
	{
		StatusCode = HttpStatusCode.BadRequest,
		Response = new
		{
			ErrorCode = "user_not_found"
		}
	})
);

// ...

app.UseExceptionHandlingMiddleware();
```

Any exceptions thrown of type `UserNotFoundException` (or any exceptions inheriting from this type) will cause the above-mentioned response to be written.

## Middleware Configuration

| Configuration | Default Value | Details |
| - | - | - |
| IgnoreTaskCancellation | true | Exceptions of type `TaskCanceledException` will be ignored, and a default response will be written instead. |
| AllowExceptionInheritance | true | Whether to look for exception base classes when matching a handler. |
| JsonSerializerOptions | null | Custom JsonSerializerOptions to use when writing the response. |
| WriteExceptionDetails | false | Whether to print the exception message & stack trace when writing the default response. |
