using EnsyNet.DataAccess.Abstractions.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EnsyNet.DataAccess.EntityFramework.Extensions;

internal static class SetPropertyCallsExtensions
{
    public static Action<UpdateSettersBuilder<T>> GetUpdateSettersAction<T>(
        this Expression<Func<EntityUpdates<T>, EntityUpdates<T>>> expression) where T : DbEntity
    {
        var setterActions = ExtractSetterActions<T>(expression);
        return builder =>
        {
            foreach (var action in setterActions)
                action(builder);
        };
    }

    private static List<Action<UpdateSettersBuilder<T>>> ExtractSetterActions<T>(
        Expression<Func<EntityUpdates<T>, EntityUpdates<T>>> expression) where T : DbEntity
    {
        var setters = new List<(LambdaExpression propertyGetter, LambdaExpression valueSetter)>();
        Expression current = expression.Body;

        while (current is MethodCallExpression methodCall)
        {
            setters.Add((UnwrapQuote(methodCall.Arguments[0]), UnwrapQuote(methodCall.Arguments[1])));
            current = methodCall.Object!;
        }

        setters.Reverse();
        return setters.Select(s => BuildSetterAction<T>(s.propertyGetter, s.valueSetter)).ToList();
    }

    private static Action<UpdateSettersBuilder<T>> BuildSetterAction<T>(
        LambdaExpression propertyGetter, LambdaExpression valueSetter)
    {
        var tProp = propertyGetter.ReturnType;
        var setPropertyMethod = typeof(UpdateSettersBuilder<T>)
            .GetMethods()
            .First(m =>
                m.Name == "SetProperty" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[1].ParameterType.IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
            .MakeGenericMethod(tProp);

        return builder => setPropertyMethod.Invoke(builder, [propertyGetter, valueSetter]);
    }

    private static LambdaExpression UnwrapQuote(Expression expr)
        => expr is UnaryExpression { NodeType: ExpressionType.Quote } unary
            ? (LambdaExpression)unary.Operand
            : (LambdaExpression)expr;
}
