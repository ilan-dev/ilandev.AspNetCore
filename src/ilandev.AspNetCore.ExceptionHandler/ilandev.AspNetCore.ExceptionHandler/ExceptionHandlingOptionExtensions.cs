using System.Text.Json;
using ilandev.AspNetCore.ExceptionHandler.Models;

namespace ilandev.AspNetCore.ExceptionHandler;

/// <summary>
///     Extensions for <see cref="ExceptionHandlingOptions" />.
/// </summary>
public static class ExceptionHandlingOptionExtensions
{
    /// <summary>
    ///     Sets the <see cref="ExceptionHandlingOptions.IgnoreTaskCancellation" /> property.
    /// </summary>
    public static ExceptionHandlingOptions IgnoreTaskCancellation(this ExceptionHandlingOptions options, bool ignoreTaskCancellation = true)
    {
        options.IgnoreTaskCancellation = ignoreTaskCancellation;

        return options;
    }

    /// <summary>
    ///     Sets the <see cref="ExceptionHandlingOptions.AllowExceptionInheritance" /> property.
    /// </summary>
    public static ExceptionHandlingOptions AllowExceptionInheritance(this ExceptionHandlingOptions options, bool allowExceptionInheritance = true)
    {
        options.AllowExceptionInheritance = allowExceptionInheritance;

        return options;
    }

        /// <summary>
    ///     Sets the <see cref="ExceptionHandlingOptions.LogExceptionDetails" /> property.
    /// </summary>
    public static ExceptionHandlingOptions LogExceptionDetails(this ExceptionHandlingOptions options, bool logExceptionDetails = true)
    {
        options.LogExceptionDetails = logExceptionDetails;

        return options;
    }

    /// <summary>
    ///     Sets the <see cref="ExceptionHandlingOptions.WriteExceptionDetails" /> property.
    /// </summary>
    public static ExceptionHandlingOptions WriteExceptionDetails(this ExceptionHandlingOptions options, bool writeExceptionDetails = false)
    {
        options.WriteExceptionDetails = writeExceptionDetails;

        return options;
    }

    /// <summary>
    ///     Sets the <see cref="ExceptionHandlingOptions.JsonSerializerOptions" /> property.
    /// </summary>
    public static ExceptionHandlingOptions JsonSerializerOptions(this ExceptionHandlingOptions options, JsonSerializerOptions? jsonSerializerOptions = default)
    {
        options.JsonSerializerOptions = jsonSerializerOptions;

        return options;
    }

    /// <summary>
    ///     Configures the <see cref="ExceptionHandlingOptions.JsonSerializerOptions" /> property.
    /// </summary>
    public static ExceptionHandlingOptions JsonSerializerOptions(this ExceptionHandlingOptions options, Action<JsonSerializerOptions> configureJsonOptions)
    {
        var opts = new JsonSerializerOptions();

        configureJsonOptions(opts);

        return options.JsonSerializerOptions(opts);
    }

    /// <summary>
    ///     Registers an exception handler for the given exception type.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to handle.</typeparam>
    public static ExceptionHandlingOptions AddHandler<TException>(this ExceptionHandlingOptions options, Func<TException, ExceptionMapping> handler)
        where TException : Exception
    {
        options.Handlers.Add(
            new()
            {
                ExceptionType = typeof(TException),
                Mapping = handler
            }
        );

        return options;
    }
}