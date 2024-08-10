namespace EnsyNet.Authentication.Core.Configuration;

public sealed record AuthConfig : IConfig
{
    public static string ConfigName => "Auth";

    public AuthType Type { get; init; }

    public bool IsValid()
        => true;
}

public enum AuthType
{
    None,
    Basic,
}
