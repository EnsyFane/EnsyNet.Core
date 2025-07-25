﻿using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Models;

/// <summary>
/// Class used to add updates to an entity.
/// </summary>
/// <typeparam name="T">The type of the entity to be updated.</typeparam>
[PublicAPI]
public sealed class EntityUpdates<T> where T : DbEntity
{
    /// <summary>
    /// Method used to add updates to an entity.
    /// </summary>
    /// <remarks>The method doesn't actually do anything. It is just used to create an expression that can be used by the underlying DB engine to create an SQL query.</remarks>
    /// <typeparam name="TProp">Type of the entity's property to be updated.</typeparam>
    /// <param name="propertyGetter">Function to get a property from the entity.</param>
    /// <param name="propertyValueSetter">Function that returns the value to set on the property retrieved by <paramref name="propertyGetter"/>.</param>
    /// <returns>A reference to <see cref="EntityUpdates{T}"/> so that calls can be chained.</returns>
    public EntityUpdates<T> AddUpdate<TProp>(Func<T, TProp> propertyGetter, Func<T, TProp> propertyValueSetter)
    {
        return this;
    }
}
