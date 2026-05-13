using EnsyNet.DataAccess.Abstractions.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EnsyNet.DataAccess.EntityFramework.Extensions;

internal static class SetPropertyCallsExtensions
{
    public static Action<UpdateSettersBuilder<T>> ToUpdateSettersAction<T>(
        this EntityUpdates<T> updates) where T : DbEntity
        => builder =>
        {
            foreach ((var propGetter, var valueSetter) in updates.GetUpdates())
            {
                var method = typeof(UpdateSettersBuilder<T>)
                    .GetMethods()
                    .First(m =>
                        m is { Name: "SetProperty", IsGenericMethod: true } &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType.IsGenericType &&
                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                    .MakeGenericMethod(propGetter.ReturnType);
                method.Invoke(builder, [propGetter, valueSetter]);
            }
            builder.SetProperty(t => t.UpdatedAt, DateTime.UtcNow);
            builder.SetProperty(t => t.CreatedAt, t => t.CreatedAt);
            builder.SetProperty(t => t.DeletedAt, (DateTime?)null);
        };
}
