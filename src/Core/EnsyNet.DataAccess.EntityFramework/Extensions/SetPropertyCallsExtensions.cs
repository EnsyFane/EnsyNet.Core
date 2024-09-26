using Microsoft.EntityFrameworkCore.Query;

using System.Linq.Expressions;

namespace EnsyNet.DataAccess.EntityFramework.Extensions;

internal static class SetPropertyCallsExtensions
{
    /// <summary>
    /// Merges 2 expressions into a single expression.
    /// </summary>
    /// <typeparam name="T">The type of the entity saved in the database.</typeparam>
    /// <param name="initialExpression">Left expression.</param>
    /// <param name="additionalExpression">Right expression.</param>
    /// <returns>A new expression composed from merging the 2 expressions.</returns>
    public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> AddExpression<T>(
        this Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> initialExpression,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> additionalExpression)
    {
        var body = ReplacingExpressionVisitor.Replace(additionalExpression.Parameters[0], initialExpression.Body, additionalExpression.Body);
        return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(body, initialExpression.Parameters);
    }
}
