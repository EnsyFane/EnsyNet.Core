using EnsyNet.Authentication.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace EnsyNet.Authentication.Authenticators.Abstractions.Authenticators;

public interface IAuthenticator
{
    AuthType AuthType { get; }
    Task<bool> Authenticate(HttpContext context);
}
