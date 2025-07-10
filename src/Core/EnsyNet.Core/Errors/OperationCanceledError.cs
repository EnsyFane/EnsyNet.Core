using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.Core.Errors;

/// <summary>
/// Error returned when an operation is canceled.
/// </summary>
[PublicAPI]
public sealed record OperationCanceledError : Error 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationCanceledError"/> class.
    /// </summary>
    public OperationCanceledError(OperationCanceledException e) : base(CoreErrorCodes.OperationCanceledError, e) { }
}
