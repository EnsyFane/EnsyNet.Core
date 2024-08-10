namespace EnsyNet.Authentication.Core.Models;

public sealed record Organization
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}
