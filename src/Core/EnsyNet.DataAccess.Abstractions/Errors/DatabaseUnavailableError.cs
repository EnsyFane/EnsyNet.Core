using EnsyNet.Core.Results;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database operation fails because the database could not be reached
/// (connection refused, timeout, server down, etc.).
/// </summary>
[PublicAPI]
public sealed record DatabaseUnavailableError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUnavailableError"/> class.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    public DatabaseUnavailableError(Exception exception) : base(ErrorCodes.DatabaseUnavailableError, exception) { }
}
