using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database entity was not found.
/// </summary>
public sealed record EntityNotFoundError : Error
{
    public EntityNotFoundError() : base(ErrorCodes.ENTITY_NOT_FOUND_ERROR)
    {
    }
}
