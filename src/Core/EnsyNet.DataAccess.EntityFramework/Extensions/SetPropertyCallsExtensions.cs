using EnsyNet.DataAccess.Abstractions.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;

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

    public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> GetSetPropertyCallsExpression<T>(this Expression<Func<EntityUpdates<T>, EntityUpdates<T>>> expression) where T : DbEntity
    {
        var methods = SplitMethodChain(expression);
        var setPropertyCalls = methods.Select(MorphExpression<T>);
        var mergedExpression = setPropertyCalls.Aggregate((x, y) => x.AddExpression(y));
        return mergedExpression;
    }

    private static Expression<Func<T, T>>[] SplitMethodChain<T>(Expression<Func<T, T>> expression)
    {
        var methodCalls = new List<(MethodInfo method, IEnumerable<Expression> arguments)>();
        Expression currentExpression = expression.Body;

        while (currentExpression is MethodCallExpression methodCall)
        {
            methodCalls.Add((methodCall.Method, methodCall.Arguments));
            currentExpression = methodCall.Object!;
        }
        methodCalls.Reverse();

        var head = currentExpression;
        var methods = methodCalls.Select(m => Expression.Call(head, m.method, m.arguments)).Select(m => Expression.Lambda<Func<T, T>>(m, expression.Parameters[0])).ToArray();
        return methods;
    }

    private static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> MorphExpression<T>(Expression<Func<EntityUpdates<T>, EntityUpdates<T>>> expression) where T : DbEntity
    {
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyExpressionTemplate = x => x.SetProperty(e => (dynamic)null!, e => null!);
        var entityFrameworkSetPropertyMethod = (setPropertyExpressionTemplate.Body as MethodCallExpression)!.Method;
        var expressionEntityUpdateArguments = (expression.Body as MethodCallExpression)!.Arguments;
        var expressionEntityUpdateType = expressionEntityUpdateArguments[1].Type.GetGenericArguments()[1];

        var entityFrameworkSetPropertyGenericMethod = entityFrameworkSetPropertyMethod.DeclaringType!
            .GetMethods()
            .Where(x => x.Name == entityFrameworkSetPropertyMethod.Name)
            .ToArray()[0] // Find better way to get correct method
            .MakeGenericMethod(expressionEntityUpdateType);

        var methodCall = Expression.Call(setPropertyExpressionTemplate.Parameters[0], entityFrameworkSetPropertyGenericMethod, expressionEntityUpdateArguments);
        var lambdaExpression = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(methodCall, setPropertyExpressionTemplate.Parameters[0]);
        return lambdaExpression;
    }
}
