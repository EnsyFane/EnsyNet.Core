using JetBrains.Annotations;

namespace EnsyNet.Core.Results;

/// <summary>
/// Wrapper record used as a return type for methods that can fail.
/// </summary>
/// <remarks>
/// Used to avoid using exceptions for flow control.
/// If the method fails, the <see cref="Error"/> property will be populated.
/// </remarks>
[PublicAPI]
public record Result
{
    /// <summary>
    /// The error that occurred during the operation. (If applicable)
    /// </summary>
    public Error? Error { get; private init; }

    /// <summary>
    /// Whether the operation completed with errors or not.
    /// </summary>
    public bool HasError => Error is not null;
    
    internal Result() { }

    /// <summary>
    /// Gets a new <see cref="Result"/> instance with no errors.
    /// </summary>
    /// <returns>A <see cref="Result"/> instance with no errors.</returns>
    public static Result Ok() => new();

    /// <summary>
    /// Gets a new <see cref="Result{T}"/> instance with no errors. And with data that can be used by the caller method.
    /// </summary>
    /// <typeparam name="T">The type of the data stored in the result</typeparam>
    /// <param name="data">The actual data to be returned</param>
    /// <returns>A <see cref="Result{T}"/> instance with no errors.</returns>
    public static Result<T> Ok<T>(T data) => new() { Data = data };

    /// <summary>
    /// Gets a new <see cref="Result"/> instance with the specified error.
    /// </summary>
    /// <param name="error">The specified error.</param>
    /// <returns>A <see cref="Result"/> instance with an error.</returns>
    public static Result FromError(Error error) => new() { Error = error };

    /// <summary>
    /// Gets a new <see cref="Result"/> instance with the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the data stored in the result</typeparam>
    /// <param name="error">The specified error.</param>
    /// <returns>A <see cref="Result{T}"/> instance with an error.</returns>
    public static Result<T> FromError<T>(Error error) => new() { Error = error };
}

/// <inheritdoc />
/// <typeparam name="T">The type of the data stored in the result.</typeparam>
public sealed record Result<T> : Result
{
    /// <summary>
    /// The data returned by the operation. (If applicable)
    /// </summary>
    public T? Data { get; init; }
}
