namespace ilandev.AspNetCore.ExceptionHandler.Models;

internal class ExceptionDetailsResponse
{
    public required string Message { get; set; }
    public string? StackTrace { get; set; }
}