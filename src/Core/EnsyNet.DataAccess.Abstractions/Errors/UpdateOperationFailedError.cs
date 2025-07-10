using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk update operation fails.
/// </summary>
public sealed record UpdateOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOperationFailedError"/> class.
    /// </summary>
    public UpdateOperationFailedError() : base(ErrorCodes.UpdateOperationFailedError) { }
}
