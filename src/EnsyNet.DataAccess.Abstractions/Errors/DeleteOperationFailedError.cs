using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a delete operation fails.
/// </summary>
public sealed record DeleteOperationFailedError : Error
{
    public DeleteOperationFailedError() : base(ErrorCodes.DELETE_OPERATION_FAILED_ERROR)
    {
    }
}
