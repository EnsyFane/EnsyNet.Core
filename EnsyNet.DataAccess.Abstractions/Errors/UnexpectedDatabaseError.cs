using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when an unexpected/unknown database exception is thrown.
/// </summary>
public sealed record UnexpectedDatabaseError : Error
{
    public UnexpectedDatabaseError(Exception exception) : base(ErrorCodes.UNEXPECTED_DATABASE_ERROR, exception)
    {
    }
}
