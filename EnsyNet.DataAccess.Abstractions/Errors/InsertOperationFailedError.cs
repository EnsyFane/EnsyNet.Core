using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record InsertOperationFailedError : Error
{
    public InsertOperationFailedError(Exception exception) : base(ErrorCodes.INSERT_OPERATION_FAILED_ERROR, exception)
    {
    }
}
