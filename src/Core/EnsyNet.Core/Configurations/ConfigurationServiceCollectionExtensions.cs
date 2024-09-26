using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnsyNet.Core.Configurations;

public static class ConfigurationServiceCollectionExtensions
{
    /// <summary>
    /// Add configuration <typeparamref name="T"/> to the Dependency Injection container.
    /// </summary>
    /// <typeparam name="T">The configuration type to add to Dependency Injection container.</typeparam>
    /// <param name="services">Service collection from Dependency Injection container.</param>
    /// <param name="configuration">Configuration conaining all config values.</param>
    /// <returns>The service collection so that it can be chained in subsequent calls.</returns>
    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class, IConfig
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(T.ConfigName))
            .Validate(config => config.IsValid(), $"Invalid {T.ConfigName} configuration.")
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Add configuration <typeparamref name="T"/> to the Dependency Injection container.
    /// </summary>
    /// <typeparam name="T">The configuration type to add to Dependency Injection container.</typeparam>
    /// <param name="services">Service collection from Dependency Injection container.</param>
    /// <param name="configuration">Configuration conaining all config values.</param>
    /// <param name="config">The populated configuration object.</param>
    /// <returns>The service collection so that it can be chained in subsequent calls.</returns>
    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration, out T config) where T : class, IConfig
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(T.ConfigName))
            .Validate(config => config.IsValid(), $"Invalid {T.ConfigName} configuration.")
            .ValidateOnStart();

        config = configuration.GetSection(T.ConfigName).Get<T>()!;
        return services;
    }

    /// <summary>
    /// Add required configuration <typeparamref name="T"/> to the Dependency Injection container.
    /// </summary>
    /// <remarks>Throws <see cref="InvalidOperationException"/> if configuration section is not found.</remarks>
    /// <typeparam name="T">The configuration type to add to Dependency Injection container.</typeparam>
    /// <param name="services">Service collection from Dependency Injection container.</param>
    /// <param name="configuration">Configuration conaining all config values.</param>
    /// <returns>The service collection so that it can be chained in subsequent calls.</returns>
    /// <exception cref="InvalidOperationException" />
    public static IServiceCollection AddRequiredConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class, IConfig
    {
        services.AddOptions<T>()
            .Bind(configuration.GetRequiredSection(T.ConfigName))
            .Validate(config => config.IsValid(), $"Invalid {T.ConfigName} configuration.")
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Add required configuration <typeparamref name="T"/> to the Dependency Injection container.
    /// </summary>
    /// <remarks>Throws <see cref="InvalidOperationException"/> if configuration section is not found.</remarks>
    /// <typeparam name="T">The configuration type to add to Dependency Injection container.</typeparam>
    /// <param name="services">Service collection from Dependency Injection container.</param>
    /// <param name="configuration">Configuration conaining all config values.</param>
    /// <param name="config">The populated configuration object.</param>
    /// <returns>The service collection so that it can be chained in subsequent calls.</returns>
    /// <exception cref="InvalidOperationException" />
    public static IServiceCollection AddRequiredConfiguration<T>(this IServiceCollection services, IConfiguration configuration, out T config) where T : class, IConfig
    {
        services.AddOptions<T>()
            .Bind(configuration.GetRequiredSection(T.ConfigName))
            .Validate(config => config.IsValid(), $"Invalid {T.ConfigName} configuration.")
            .ValidateOnStart();

        config = configuration.GetRequiredSection(T.ConfigName).Get<T>()!;
        return services;
    }
}
