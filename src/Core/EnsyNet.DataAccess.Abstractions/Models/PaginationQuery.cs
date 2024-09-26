namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Wrapper for pagination query parameters.
/// </summary>
public sealed record PaginationQuery
{
    /// <summary>
    /// The number of items to skip in the query.
    /// </summary>
    public required int Skip { get; init; }

    /// <summary>
    /// The number of items to take in the query.
    /// </summary>
    public required int Take { get; init; }
}
