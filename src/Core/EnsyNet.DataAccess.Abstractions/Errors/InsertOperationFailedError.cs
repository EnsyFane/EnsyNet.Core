using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when an insert operation fails.
/// </summary>
public sealed record InsertOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertOperationFailedError"/> class.
    /// </summary>
    public InsertOperationFailedError() : base(ErrorCodes.INSERT_OPERATION_FAILED_ERROR) { }
}
