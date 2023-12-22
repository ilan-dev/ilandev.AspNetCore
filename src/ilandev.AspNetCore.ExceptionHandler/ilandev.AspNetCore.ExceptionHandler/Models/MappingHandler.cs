namespace ilandev.AspNetCore.ExceptionHandler.Models;

internal class MappingHandler
{
    public required Type ExceptionType { get; init; }
    public required Delegate Mapping { get; init; }
}