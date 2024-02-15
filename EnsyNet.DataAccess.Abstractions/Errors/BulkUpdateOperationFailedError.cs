using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk update operation fails.
/// </summary>
public sealed record BulkUpdateOperationFailedError : Error
{
    public BulkUpdateOperationFailedError() : base(ErrorCodes.BULK_UPDATE_OPERATION_FAILED_ERROR)
    {
    }
}
