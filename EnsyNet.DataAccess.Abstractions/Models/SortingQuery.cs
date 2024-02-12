namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Wrapper for sorting query parameters.
/// </summary>
/// <typeparam name="T">The type of the object that will be sorted.</typeparam>
/// <typeparam name="TKey">The type of the field on which the sorting will be performed.</typeparam>
public sealed record SortingQuery<T, TKey> where T : DbEntity
{
    public required Func<T, TKey> SortFieldSelector { get; init; }
    public bool Ascending { get; init; } = true;
}
