using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record BulkInsertOperationFailedError : Error
{
    public BulkInsertOperationFailedError(Exception exception) : base(ErrorCodes.BULK_INSERT_OPERATION_FAILED_ERROR, exception)
    {
    }
}
