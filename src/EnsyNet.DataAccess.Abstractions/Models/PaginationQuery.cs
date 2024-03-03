namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Wrapper for pagination query parameters.
/// </summary>
public sealed record PaginationQuery
{
    public required int Skip { get; init; }
    public required int Take { get; init; }
}
