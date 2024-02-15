using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk update operation fails.
/// </summary>
public sealed record UpdateOperationFailedError : Error
{
    public UpdateOperationFailedError() : base(ErrorCodes.UPDATE_OPERATION_FAILED_ERROR)
    {
    }
}
