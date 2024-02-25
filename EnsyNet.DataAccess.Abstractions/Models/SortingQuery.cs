using System.Linq.Expressions;

namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Wrapper for sorting query parameters.
/// </summary>
/// <typeparam name="T">The type of the object that will be sorted.</typeparam>
/// <typeparam name="TKey">The type of the field on which the sorting will be performed.</typeparam>
public sealed record SortingQuery<T> where T : DbEntity
{
    public required Expression<Func<T, object>> SortFieldSelector { get; init; }
    public bool IsAscending { get; init; } = true;
}
