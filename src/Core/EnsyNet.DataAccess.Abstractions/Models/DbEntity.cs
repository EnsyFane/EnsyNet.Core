using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Record that can be stored in the database.
/// </summary>
/// <remarks>
/// If <see cref="Id"/> is not set when inserting, it will be set to a random <see cref="Guid"/>.
/// </remarks>
[PublicAPI]
public abstract record DbEntity
{
    /// <summary>
    /// The id of the entity.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The date and time the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// The date and time the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// The date and time the entity was soft-deleted.
    /// </summary>
    /// <remarks>If null then entity is not soft deleted yet.</remarks>
    public DateTime? DeletedAt { get; init; }
}
