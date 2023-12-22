namespace ilandev.AspNetCore.ExceptionHandler.Models;

internal class MappingHandler
{
    public Type ExceptionType { get; set; }
    public Delegate Mapping { get; set; }
}
