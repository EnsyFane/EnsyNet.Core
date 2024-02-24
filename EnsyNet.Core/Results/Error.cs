namespace EnsyNet.Core.Results;

/// <summary>
/// POCO record used to represent an error.
/// </summary>
public abstract record Error
{
    public string ErrorCode { get; init; }
    public Exception? Exception { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;

    protected Error(string errorCode)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
    }

    protected Error(string errorCode, Exception exception)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        ErrorMessage = exception.Message;
    }

    protected Error(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }

    protected Error(string errorCode, Exception exception, string errorMessage)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }
}
