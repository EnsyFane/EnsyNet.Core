namespace EnsyNet.Core.Results;

/// <summary>
/// Wrapper record used as a return type for methods that can fail.
/// </summary>
/// <remarks>
/// Used to avoid using exceptions for flow control.
/// If the method fails, the <see cref="Error"/> property will be populated.
/// </remarks>
public record Result
{
    public Error? Error { get; init; }

    public bool HasError => Error is not null;
    
    protected Result() { }

    public static Result Ok() => new();
    public static Result<T> Ok<T>(T data) => new() { Data = data };
    public static Result FromError(Error error) => new() { Error = error };
    public static Result<T> FromError<T>(Error error) => new() { Error = error };
}

/// <inheritdoc />
/// <typeparam name="T">The type of the data stored in the result.</typeparam>
public sealed record Result<T> : Result
{
    public T? Data { get; init; }

    internal Result() { }
}
