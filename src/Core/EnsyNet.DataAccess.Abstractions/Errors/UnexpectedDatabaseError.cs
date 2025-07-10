using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when an unexpected/unknown database exception is thrown.
/// </summary>
[PublicAPI]
public sealed record UnexpectedDatabaseError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedDatabaseError"/> class.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    public UnexpectedDatabaseError(Exception exception) : base(ErrorCodes.UnexpectedDatabaseError, exception) { }
}
