using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk delete operation fails.
/// </summary>
public sealed record BulkDeleteOperationFailedError : Error
{
    public BulkDeleteOperationFailedError() : base(ErrorCodes.BULK_DELETE_OPERATION_FAILED_ERROR)
    {
    }
}
