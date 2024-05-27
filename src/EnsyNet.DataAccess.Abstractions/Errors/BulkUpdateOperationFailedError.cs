using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk update operation fails.
/// </summary>
public sealed record BulkUpdateOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkUpdateOperationFailedError"/> class.
    /// </summary>
    public BulkUpdateOperationFailedError() : base(ErrorCodes.BULK_UPDATE_OPERATION_FAILED_ERROR)
    {
    }
}
