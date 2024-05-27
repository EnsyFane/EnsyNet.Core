using EnsyNet.Core.Results;

namespace EnsyNet.Core.Errors;

/// <summary>
/// Error returned when an operation is canceled.
/// </summary>
public sealed record OperationCanceledError : Error 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationCanceledError"/> class.
    /// </summary>
    public OperationCanceledError(OperationCanceledException e) : base(CoreErrorCodes.OPERATION_CANCELED_ERROR) { }
}
