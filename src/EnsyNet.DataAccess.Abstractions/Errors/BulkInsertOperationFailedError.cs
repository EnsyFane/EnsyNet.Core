using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk insert operation fails.
/// </summary>
public sealed record BulkInsertOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkInsertOperationFailedError"/> class.
    /// </summary>
    public BulkInsertOperationFailedError() : base(ErrorCodes.BULK_INSERT_OPERATION_FAILED_ERROR) { }
}
