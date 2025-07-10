using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk delete operation fails.
/// </summary>
[PublicAPI]
public sealed record BulkDeleteOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkDeleteOperationFailedError"/> class.
    /// </summary>
    public BulkDeleteOperationFailedError() : base(ErrorCodes.BulkDeleteOperationFailedError) { }
}
