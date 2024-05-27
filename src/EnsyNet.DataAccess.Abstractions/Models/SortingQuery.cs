using System.Linq.Expressions;

namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Wrapper for sorting query parameters.
/// </summary>
/// <typeparam name="T">The type of the object that will be sorted.</typeparam>
public sealed record SortingQuery<T> where T : DbEntity
{
    /// <summary>
    /// Expression that selects the field to sort by.
    /// </summary>
    public required Expression<Func<T, object>> SortFieldSelector { get; init; }
    /// <summary>
    /// Whether the sorting is ascending or descending.
    /// </summary>
    public bool IsAscending { get; init; } = true;
}
