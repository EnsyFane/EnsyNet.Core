using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnsyNet.Authentication.Core.Configuration;

public interface IConfig
{
    static abstract string ConfigName { get; }

    bool IsValid();
}

public static class ConfigServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : class, IConfig
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(T.ConfigName))
            .Validate(config => config.IsValid(), $"Invalid {T.ConfigName} configuration.")
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration, out T config) where T : class, IConfig
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(T.ConfigName))
            .Validate(config => config.IsValid(), $"Invalid {T.ConfigName} configuration.")
            .ValidateOnStart();

        config = configuration.GetSection(T.ConfigName).Get<T>()!;
        return services;
    }
}
