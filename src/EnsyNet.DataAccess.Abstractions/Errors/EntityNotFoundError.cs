using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database entity was not found.
/// </summary>
public sealed record EntityNotFoundError<T> : Error where T : DbEntity
{
    public EntityNotFoundError() : base(ErrorCodes.ENTITY_NOT_FOUND_ERROR, $"Entity of type {typeof(T).Name} not found in the database.")
    {
    }
}
