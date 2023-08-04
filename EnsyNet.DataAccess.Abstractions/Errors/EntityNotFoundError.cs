using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record EntityNotFoundError : Error
{
    public EntityNotFoundError() : base(ErrorCodes.ENTITY_NOT_FOUND_ERROR)
    {
    }
}
