using EnsyNet.Authentication.Authenticators.Abstractions.Attributes;

using Microsoft.AspNetCore.Mvc;

namespace EnsyNet.Authentication.Api.Controllers;

[ApiController]
[Route("api/controller")]
public class AuthProtectedController : ControllerBase
{
    [EnsyNetAuthenticate]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
