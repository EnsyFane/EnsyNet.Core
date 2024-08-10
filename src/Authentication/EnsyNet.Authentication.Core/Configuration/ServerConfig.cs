namespace EnsyNet.Authentication.Core.Configuration;

public sealed record ServerConfig : IConfig
{
    public static string ConfigName => "Server";

    public bool ShouldGenerateSwagger { get; init; } = true;

    public bool IsValid()
        => true;
}
