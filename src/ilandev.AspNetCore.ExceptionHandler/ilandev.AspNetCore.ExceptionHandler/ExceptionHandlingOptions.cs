using System.Text.Json;
using ilandev.AspNetCore.ExceptionHandler.Models;

namespace ilandev.AspNetCore.ExceptionHandler;

/// <summary>
///     Options for the exception handling middleware.
/// </summary>
public class ExceptionHandlingOptions
{
    /// <summary>
    ///     Whether to ignore <see cref="TaskCanceledException" />.
    /// </summary>
    public bool IgnoreTaskCancellation { get; set; } = true;

    /// <summary>
    ///     Whether to match handlers that are assignable to the caught exception type.
    /// </summary>
    public bool AllowExceptionInheritance { get; set; } = true;

    /// <summary>
    ///     The <see cref="System.Text.Json.JsonSerializerOptions" /> to use when serializing the response.
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    ///     Whether to write the exception details in the response when writing a default response.
    /// </summary>
    public bool WriteExceptionDetails { get; set; }

    internal List<MappingHandler> Handlers { get; set; } = [];
}