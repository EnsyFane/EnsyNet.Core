using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database operation fails because it would violate a foreign key constraint.
/// </summary>
[PublicAPI]
public sealed record ForeignKeyConstraintViolationError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyConstraintViolationError"/> class.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    public ForeignKeyConstraintViolationError(Exception exception) : base(ErrorCodes.ForeignKeyConstraintViolationError, exception) { }
}
