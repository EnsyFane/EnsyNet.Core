using EnsyNet.Authentication.Authenticators.Abstractions.Authenticators;
using EnsyNet.Authentication.Core.Configuration;

using Microsoft.AspNetCore.Http;

using System.Text;

namespace EnsyNet.Authentication.Authenticators.BasicAuth;

internal sealed class BasicAuthenticator : IAuthenticator
{
    public AuthType AuthType => AuthType.Basic;

    public Task<bool> Authenticate(HttpContext context)
    {
        var hasAuthorizationHeader = context.Request.Headers.TryGetValue("Authorization", out var authHeader);
        if (!hasAuthorizationHeader)
        {
            context.Response.Headers.Add("WWW-Authenticate", "Basic");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return Task.FromResult(false);
        }

        var authHeaderString = authHeader.ToString();
        var auth = authHeaderString.Trim().Split(' ');
        if (auth.Length != 2 || auth[0] != "Basic")
        {
            context.Response.Headers.Add("WWW-Authenticate", "Basic");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult(false);
        }

        var authCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(auth[1])).Trim().Split(':');
        if (authCredentials.Length != 2)
        {
            context.Response.Headers.Add("WWW-Authenticate", "Basic");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult(false);
        }

        if (authCredentials[0] != "user" || authCredentials[1] != "password")
        {
            context.Response.Headers.Add("WWW-Authenticate", "Basic");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
}
