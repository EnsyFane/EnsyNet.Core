using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk update operation fails.
/// </summary>
[PublicAPI]
public sealed record UpdateOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOperationFailedError"/> class.
    /// </summary>
    public UpdateOperationFailedError() : base(ErrorCodes.UpdateOperationFailedError) { }
}
