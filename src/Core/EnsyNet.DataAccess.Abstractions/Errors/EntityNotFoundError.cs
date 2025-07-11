﻿using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when a database entity was not found.
/// </summary>
/// <typeparam name="T">The type of the entity that was not found.</typeparam>
[PublicAPI]
public sealed record EntityNotFoundError<T> : Error where T : DbEntity
{
    /// <summary>
    /// Message template used to generate the <see cref="Error.ErrorMessage"/>.
    /// </summary>
    public const string MessageTemplate = "Entity of type {0} not found in the database.";

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundError{T}"/> class.
    /// </summary>
    public EntityNotFoundError() : base(ErrorCodes.EntityNotFoundError, string.Format(MessageTemplate, typeof(T).Name)) { }
}
