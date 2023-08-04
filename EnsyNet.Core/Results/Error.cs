namespace EnsyNet.Core.Results;

/// <summary>
/// POCO record used to represent an error.
/// </summary>
public abstract record Error
{
    public string ErrorCode { get; init; }
    public Exception? Exception { get; init; }

    protected Error(string errorCode, Exception? exception = null)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        Exception = exception;
    }
}
