using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk update operation fails.
/// </summary>
[PublicAPI]
public sealed record BulkUpdateOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkUpdateOperationFailedError"/> class.
    /// </summary>
    public BulkUpdateOperationFailedError() : base(ErrorCodes.BulkUpdateOperationFailedError) { }
}
