using System.Net;

namespace ilandev.AspNetCore.ExceptionHandler.Models;

/// <summary>
///     A response mapping.
/// </summary>
public class ExceptionMapping
{
    /// <summary>
    ///     Gets or sets the <see cref="HttpStatusCode" /> property on the response.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    ///     Gets or sets the response body.
    /// </summary>
    public object? Response { get; set; }
}