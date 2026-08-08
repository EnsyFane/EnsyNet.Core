using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database operation fails because it would violate a unique constraint/index.
/// </summary>
[PublicAPI]
public sealed record UniqueConstraintViolationError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraintViolationError"/> class.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    public UniqueConstraintViolationError(Exception exception) : base(ErrorCodes.UniqueConstraintViolationError, exception) { }
}
