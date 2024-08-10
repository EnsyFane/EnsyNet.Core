using EnsyNet.Authentication.Api;
using EnsyNet.Authentication.Authenticators.Abstractions;
using EnsyNet.Authentication.Authenticators.BasicAuth;
using EnsyNet.Authentication.Core.Configuration;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddServerConfig(builder.Configuration)
    .AddEnsyNetAuthentication(builder.Configuration)
    .AddBasicAuth();

var app = builder.Build();

var serverConfig = app.Services.GetRequiredService<IOptions<ServerConfig>>().Value;
if (serverConfig.ShouldGenerateSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseEnsyNetAuthentication();
app.Run();
