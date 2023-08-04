using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record BulkDeleteOperationFailedError : Error
{
    public BulkDeleteOperationFailedError(Exception exception) : base(ErrorCodes.BULK_DELETE_OPERATION_FAILED_ERROR, exception)
    {
    }
}
