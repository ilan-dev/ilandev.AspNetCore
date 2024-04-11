namespace ilandev.Extensions.Configuration;

/// <summary>
/// Common interface for application options.
/// </summary>
public interface IAppOptions
{
    /// <summary>
    /// The configuration section for this options model.
    /// </summary>
    static abstract string Section { get; }
}
