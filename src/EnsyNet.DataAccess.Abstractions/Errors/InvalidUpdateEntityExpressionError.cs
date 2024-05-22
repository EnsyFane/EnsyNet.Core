using EnsyNet.Core.Results;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when an update entity expression is invalid.
/// This can be because the expression is not supported by the repository or managed fields were updated.
/// </summary>
public sealed record InvalidUpdateEntityExpressionError : Error
{
    public InvalidUpdateEntityExpressionError() : base(ErrorCodes.INVALID_UPDATE_ENTITY_EXPRESSION_ERROR)
    {
    }
}
