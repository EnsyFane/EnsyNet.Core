using JetBrains.Annotations;

namespace EnsyNet.Core.Results;

/// <summary>
/// POCO record used to represent an error.
/// </summary>
[PublicAPI]
public abstract record Error
{
    /// <summary>
    /// The error code for the error.
    /// </summary>
    /// <remarks>Usually in the format "[ShortErrorCode]".</remarks>
    public string ErrorCode { get; init; }

    /// <summary>
    /// The exception that caused the error. (If applicable)
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// A description of the error.
    /// </summary>
    public string ErrorMessage { get; private init; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="errorCode">The <see cref="ErrorCode"/> of the error.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="errorCode"/> is null.</exception>
    protected Error(string errorCode)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode">The <see cref="ErrorCode"/> of the error.</param>
    /// <param name="exception">The <see cref="Exception"/> of the error.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="errorCode"/> or <paramref name="exception"/> is null.</exception>
    protected Error(string errorCode, Exception exception)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        ErrorMessage = exception.Message;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode">The <see cref="ErrorCode"/> of the error.</param>
    /// <param name="errorMessage">The <see cref="ErrorMessage"/> of the error.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="errorCode"/> or <paramref name="errorMessage"/> is null.</exception>
    protected Error(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode">The <see cref="ErrorCode"/> of the error.</param>
    /// <param name="exception">The <see cref="Exception"/> of the error.</param>
    /// <param name="errorMessage">The <see cref="ErrorMessage"/> of the error.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="errorCode"/>, <paramref name="exception"/> or <paramref name="errorMessage"/> is null.</exception>
    protected Error(string errorCode, Exception exception, string errorMessage)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }
}
