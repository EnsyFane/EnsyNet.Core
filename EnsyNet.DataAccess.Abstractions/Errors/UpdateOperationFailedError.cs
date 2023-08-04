using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record UpdateOperationFailedError : Error
{
    public UpdateOperationFailedError(Exception exception) : base(ErrorCodes.UPDATE_OPERATION_FAILED_ERROR, exception)
    {
    }
}
