using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record DeleteOperationFailedError : Error
{
    public DeleteOperationFailedError(Exception exception) : base(ErrorCodes.DELETE_OPERATION_FAILED_ERROR, exception)
    {
    }
}
