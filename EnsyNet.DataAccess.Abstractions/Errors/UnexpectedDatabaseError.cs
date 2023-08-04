using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

public sealed record UnexpectedDatabaseError : Error
{
    public UnexpectedDatabaseError(Exception exception) : base(ErrorCodes.UNEXPECTED_DATABASE_ERROR, exception)
    {
    }
}
