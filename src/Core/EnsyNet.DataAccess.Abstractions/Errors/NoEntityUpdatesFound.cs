using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.Abstractions.Errors;

/// <summary>
/// Error returned when the user tried to apply no updates to an entity.
/// </summary>
/// <typeparam name="T">The type of the entity for which updates should have been applied.</typeparam>
public sealed record NoEntityUpdatesFound<T> : Error where T : DbEntity
{
    /// <summary>
    /// Message template used to generate the <see cref="Error.ErrorMessage"/>.
    /// </summary>
    public const string MessageTemplate = "No updates found for entity of type {0}.";

    /// <summary>
    /// Initializes a new instance of the <see cref="NoEntityUpdatesFound{T}"/> class.
    /// </summary>
    public NoEntityUpdatesFound() : base(ErrorCodes.NO_ENTITY_UPDATES_FOUND, string.Format(MessageTemplate, typeof(T).Name)) { }
}
