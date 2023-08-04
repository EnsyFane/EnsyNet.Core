namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Record that can be stored in the database.
/// </summary>
/// <remarks>
/// If <see cref="Id"/> is not set when inserting, it will be set to a random <see cref="Guid"/>.
/// </remarks>
public abstract record DbEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
