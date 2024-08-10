using EnsyNet.Authentication.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace EnsyNet.Authentication.Authenticators.Abstractions.Authenticators;

internal sealed class NoOpAuthenticator : IAuthenticator
{
    public AuthType AuthType => AuthType.None;

    public Task<bool> Authenticate(HttpContext context)
        => Task.FromResult(true);
}
