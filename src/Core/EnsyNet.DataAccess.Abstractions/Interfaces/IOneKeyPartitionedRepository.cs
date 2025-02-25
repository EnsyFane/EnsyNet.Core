using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.Abstractions.Interfaces;

/// <summary>
/// Repository containing all supported operations that can be performed on a database entity.
/// </summary>
/// <typeparam name="T">The type of the entity stored in the database.</typeparam>
public interface IOneKeyPartitionedRepository<T> : IRepository<T> where T : DbEntity
{
    /// <summary>
    /// Retrieves a single entity from the database by its <see cref="DbEntity.Id"/> if it is in the provided partition.
    /// </summary>
    /// <param name="partition">The partition by which to filter out entities.</param>
    /// <param name="id">The id of the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError{T}"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetById(object partition, Guid id, CancellationToken ct);
}
