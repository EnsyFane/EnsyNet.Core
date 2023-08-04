using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record BulkUpdateOperationFailedError : Error
{
    public BulkUpdateOperationFailedError(Exception exception) : base(ErrorCodes.BULK_UPDATE_OPERATION_FAILED_ERROR, exception)
    {
    }
}
