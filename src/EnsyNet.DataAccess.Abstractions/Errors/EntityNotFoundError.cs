using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database entity was not found.
/// </summary>
/// <typeparam name="T">The type of the entity that was not found.</typeparam>
public sealed record EntityNotFoundError<T> : Error where T : DbEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundError{T}"/> class.
    /// </summary>
    public EntityNotFoundError() : base(ErrorCodes.ENTITY_NOT_FOUND_ERROR, $"Entity of type {typeof(T).Name} not found in the database.")
    {
    }
}
