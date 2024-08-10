using EnsyNet.Authentication.Authenticators.Abstractions.Authenticators;
using EnsyNet.Authentication.Authenticators.Abstractions.Middleware;
using EnsyNet.Authentication.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnsyNet.Authentication.Authenticators.Abstractions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnsyNetAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguration<AuthConfig>(configuration);
        services.AddSingleton<IAuthenticator, NoOpAuthenticator>();
        services.AddScoped<AuthenticationMiddleware>();

        return services;
    }

    public static IApplicationBuilder UseEnsyNetAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<AuthenticationMiddleware>();

        return app;
    }
}
