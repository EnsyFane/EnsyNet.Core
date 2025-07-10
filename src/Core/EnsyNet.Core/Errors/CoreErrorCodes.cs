using JetBrains.Annotations;

namespace EnsyNet.Core.Errors;

/// <summary>
/// Error codes used in the core layer.
/// </summary>
[PublicAPI]
public static class CoreErrorCodes
{
    /// <summary>
    /// Error code for when an operation was canceled via a <see cref="CancellationToken"/>.
    /// </summary>
    public const string OperationCanceledError = "[OperationCanceledError]";
}
