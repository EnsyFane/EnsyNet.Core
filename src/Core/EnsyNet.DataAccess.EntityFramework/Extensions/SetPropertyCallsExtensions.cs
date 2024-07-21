using Microsoft.EntityFrameworkCore.Query;

using System.Linq.Expressions;

namespace EnsyNet.DataAccess.EntityFramework.Extensions;

internal static class SetPropertyCallsExtensions
{
    public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> AddExpression<T>(
        this Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> initialExpression,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> additionalExpression)
    {
        var body = ReplacingExpressionVisitor.Replace(additionalExpression.Parameters[0], initialExpression.Body, additionalExpression.Body);
        return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(body, initialExpression.Parameters);
    }
}
