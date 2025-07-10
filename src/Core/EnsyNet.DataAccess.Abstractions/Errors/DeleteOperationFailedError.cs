using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a delete operation fails.
/// </summary>
[PublicAPI]
public sealed record DeleteOperationFailedError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteOperationFailedError"/> class.
    /// </summary>
    public DeleteOperationFailedError() : base(ErrorCodes.DeleteOperationFailedError) { }
}
