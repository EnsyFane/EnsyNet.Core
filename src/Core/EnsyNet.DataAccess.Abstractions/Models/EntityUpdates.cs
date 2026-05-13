using JetBrains.Annotations;

using System.Linq.Expressions;

namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Builder used to specify updates to apply to an entity.
/// </summary>
/// <typeparam name="T">The type of the entity to be updated.</typeparam>
[PublicAPI]
public sealed class EntityUpdates<T> where T : DbEntity
{
    private static readonly HashSet<string> _protectedProperties = ["Id", "CreatedAt", "UpdatedAt", "DeletedAt"];
    private readonly List<(LambdaExpression PropertyGetter, LambdaExpression ValueSetter)> _updates = [];

    /// <summary>
    /// Whether any call to <see cref="AddUpdate{TProp}"/> targeted a protected property (<c>Id</c>, <c>CreatedAt</c>, <c>UpdatedAt</c>, <c>DeletedAt</c>).
    /// </summary>
    public bool HasProtectedPropertyUpdate { get; private set; }

    /// <summary>
    /// Adds an update for the property selected by <paramref name="propertyGetter"/>.
    /// </summary>
    /// <typeparam name="TProp">Type of the property to update.</typeparam>
    /// <param name="propertyGetter">Selects the property to update.</param>
    /// <param name="valueSetter">Returns the new value for the property.</param>
    /// <returns>This instance, so calls can be chained.</returns>
    public EntityUpdates<T> AddUpdate<TProp>(
        Expression<Func<T, TProp>> propertyGetter,
        Expression<Func<T, TProp>> valueSetter)
    {
        if (propertyGetter.Body is MemberExpression { Member.Name: var name }
            && _protectedProperties.Contains(name))
        {
            HasProtectedPropertyUpdate = true;
        }

        _updates.Add((propertyGetter, valueSetter));
        return this;
    }

    /// <summary>
    /// Returns the collected property/value expression pairs.
    /// </summary>
    public IReadOnlyList<(LambdaExpression PropertyGetter, LambdaExpression ValueSetter)> GetUpdates()
        => _updates;
}
