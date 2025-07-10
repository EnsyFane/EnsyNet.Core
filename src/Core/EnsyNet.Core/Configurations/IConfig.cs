using JetBrains.Annotations;

namespace EnsyNet.Core.Configurations;

/// <summary>
/// Interface for configuration classes.
/// </summary>
/// <remarks>Creating records with this interface allows the use of methods from <see cref="ConfigurationServiceCollectionExtensions"/> which make adding configurations easier.</remarks>
[PublicAPI]
public interface IConfig
{
    /// <summary>
    /// The name of the configuration. This name is used to search for the configuration section in the <see cref="Microsoft.Extensions.Configuration.IConfiguration"/> object.
    /// </summary>
    static abstract string ConfigName { get; }

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <returns>True if configuration is valid, <br/> False otherwise.</returns>
    bool IsValid();
}
