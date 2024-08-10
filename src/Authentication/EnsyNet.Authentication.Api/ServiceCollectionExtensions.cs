using EnsyNet.Authentication.Core.Configuration;

namespace EnsyNet.Authentication.Api;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddConfiguration<ServerConfig>(configuration);
    }
}
