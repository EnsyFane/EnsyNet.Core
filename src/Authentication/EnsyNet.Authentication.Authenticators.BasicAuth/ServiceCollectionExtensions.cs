using EnsyNet.Authentication.Authenticators.Abstractions.Authenticators;

using Microsoft.Extensions.DependencyInjection;

namespace EnsyNet.Authentication.Authenticators.BasicAuth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBasicAuth(this IServiceCollection services)
    {
        return services.AddSingleton<IAuthenticator, BasicAuthenticator>();
    }
}
