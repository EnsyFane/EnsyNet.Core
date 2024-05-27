using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk delete operation fails.
/// </summary>
public sealed record BulkDeleteOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkDeleteOperationFailedError"/> class.
    /// </summary>
    public BulkDeleteOperationFailedError() : base(ErrorCodes.BULK_DELETE_OPERATION_FAILED_ERROR)
    {
    }
}
