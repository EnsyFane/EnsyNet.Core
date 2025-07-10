using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a bulk insert operation fails.
/// </summary>
[PublicAPI]
public sealed record BulkInsertOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkInsertOperationFailedError"/> class.
    /// </summary>
    public BulkInsertOperationFailedError() : base(ErrorCodes.BulkInsertOperationFailedError) { }
}
