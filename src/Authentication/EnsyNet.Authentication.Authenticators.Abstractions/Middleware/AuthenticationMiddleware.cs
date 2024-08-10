using EnsyNet.Authentication.Authenticators.Abstractions.Attributes;
using EnsyNet.Authentication.Authenticators.Abstractions.Authenticators;
using EnsyNet.Authentication.Core.Configuration;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnsyNet.Authentication.Authenticators.Abstractions.Middleware;

public sealed class AuthenticationMiddleware : IMiddleware
{
    private readonly IEnumerable<IAuthenticator> _authenticators;
    private readonly AuthConfig _authConfig;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(
        IEnumerable<IAuthenticator> authenticators,
        IOptions<AuthConfig> authConfig,
        ILogger<AuthenticationMiddleware> logger)
    {
        _authenticators = authenticators ?? throw new ArgumentNullException(nameof(authenticators));
        _authConfig = authConfig?.Value ?? throw new ArgumentNullException(nameof(authConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        if (endpoint is null)
        {
            await next(context);
            return;
        }

        var attribute = endpoint.Metadata.GetMetadata<EnsyNetAuthenticateAttribute>();
        if (attribute is null)
        {
            await next(context);
            return;
        }

        var authenticator = _authenticators.FirstOrDefault(x => x.AuthType == _authConfig.Type);
        if (authenticator is null)
        {
            _logger.LogError("No authenticator found for type {AuthType}.", _authConfig.Type);
            throw new InvalidOperationException($"No authenticator found for type {_authConfig.Type}.");
        }

        var isAuthenticated = await authenticator.Authenticate(context);
        if (isAuthenticated)
        {
            await next(context);
        }
    }
}
