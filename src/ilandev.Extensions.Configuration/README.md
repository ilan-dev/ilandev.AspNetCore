# ilandev.Extensions.Configuration

[![nuget.org](https://img.shields.io/nuget/dt/ilandev.Extensions.Configuration?style=flat-square&logo=nuget)](https://www.nuget.org/packages/ilandev.Extensions.Configuration/)

Handy `IConfiguration` extensions that help you reduce boilerplate code when adding `IOptions<T>` to your service collection.

## Usage

### Adding IOptions to the service collection

1. When creating your options models, implement `IAppOptions`
2. Call `services.ConfigureAppOptions<T>()`, where `T` is the type of your options model.

Your options model will be added to the `IServiceCollection` as `IOptions<T>`, using the configuration section returned by the `Section` property in your model.

### Binding the model directly

1. When creating your options models, implement `IAppOptions`.
2. Call `configuration.GetAppOptions<T>()` to bind a new instance of your model type.

## Examples

```csharp
public class MyOptions : IAppOptions
{
  public static string Section => "MySection";

  // ...
}

builder.Services.ConfigureAppOptions<MyOptions>(builder.Configuration); // same as calling builder.Services.Configure<MyOptions>(builder.Configuration.GetSection("MySection"));

// Bind the model manually

var options = builder.Configuration.GetAppOptions<MyOptions>();
```
