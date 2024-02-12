using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.Abstractions.Interfaces;

/// <summary>
/// Repository containing all supported operations that can be performed on a database entity.
/// </summary>
/// <typeparam name="T">The type of the entity stored in the database.</typeparam>
public interface IRepository<T> where T : DbEntity
{
    /// <summary>
    /// Retrieves a single entity from the database by its ID.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetById(Guid id, CancellationToken ct);

    /// <summary>
    /// Retrieves the first entity from the database that matches the given filter.
    /// </summary>
    /// <returns>
    /// <param name="filter">The filter expression to be used for the database query.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetByExpression(Func<T, bool> filter, CancellationToken ct);

    /// <summary>
    /// Retrieves all entities from the database. 
    /// </summary>
    /// <remarks>This might be a resource intensive operation.</remarks>
    /// <returns>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetAll(CancellationToken ct);

    /// <summary>
    /// Retrieves all entities from the database sorted based on the provided <see cref="SortingQuery{T, TKey}"/>. 
    /// </summary>
    /// <remarks>This might be a resource intensive operation.</remarks>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetAll<TKey>(SortingQuery<T, TKey> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database paginated based on the provided <see cref="PaginationQuery"/>. 
    /// </summary>
    /// <remarks>Entities will be sorted ascending by <see cref="DbEntity.Id"/>.</remarks>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetMany(PaginationQuery paginationQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database paginated based on the provided <see cref="PaginationQuery"/> and sorted based on the provided <see cref="SortingQuery{T, TKey}"/>. 
    /// </summary>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetMany<TKey>(PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter. 
    /// </summary>
    /// <remarks>Entities will be sorted ascending by <see cref="DbEntity.Id"/>.</remarks>
    /// <param name="filter">The filter to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter and paginated based on the provided <see cref="PaginationQuery"/>. 
    /// </summary>
    /// <remarks>Entities will be sorted ascending by <see cref="DbEntity.Id"/>.</remarks>
    /// <param name="filter">The filter to use.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, PaginationQuery paginationQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter and sorted based on the provided <see cref="SortingQuery{T, TKey}"/>. 
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, SortingQuery<T, TKey> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter, paginated based on the provided <see cref="PaginationQuery"/> and sorted based on the provided <see cref="SortingQuery{T, TKey}"/>. 
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Inserts a single entity into the database.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/> and <see cref="DbEntity.CreatedAt"/> fields will be overwritten.</remarks>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The inserted entity or <br/>
    /// An <see cref="Errors.InsertOperationFailedError"/> if insertion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> Insert(T entity, CancellationToken ct);

    /// <summary>
    /// Inserts multiple entities into the database.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/> and <see cref="DbEntity.CreatedAt"/> fields will be overwritten and the <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields will be set to null.</remarks>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="isAtomic">If set to true then if one insertion fails then all insertions will be reverted and the operation fails, otherwise the operation will succeed even if not all entities were succesfully inserted.</param>
    /// <returns>
    /// The inserted entities or <br/>
    /// An <see cref="Errors.BulkInsertOperationFailedError"/> if <paramref name="isAtomic"/> is set to true and one insertion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, CancellationToken ct, bool isAtomic = true);

    /// <summary>
    /// Updates a single entity from the database.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/>, <see cref="DbEntity.CreatedAt"/>, <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields can not be update manually.</remarks>
    /// <param name="entity">The updated entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of updated entities or <br/>
    /// An <see cref="Errors.UpdateOperationFailedError"/> if update fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> Update(T entity, CancellationToken ct);

    /// <summary>
    /// Updates multiple entities from the database.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/>, <see cref="DbEntity.CreatedAt"/>, <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields can not be update manually.</remarks>
    /// <param name="entities">The entities to update.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="isAtomic">If set to true then if one update fails then all updates will be reverted and the operation fails, otherwise the operation will succeed even if not all entities were succesfully updated.</param>
    /// <returns>
    /// The number of updated entities or <br/>
    /// An <see cref="Errors.BulkUpdateOperationFailedError"/> if <paramref name="isAtomic"/> is set to true and one update fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> Update(IEnumerable<T> entities, CancellationToken ct, bool isAtomic = true);

    /// <summary>
    /// Soft deletes a single entity from the database based on id.
    /// </summary>
    /// <param name="id">The id of the entity to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.DeleteOperationFailedError"/> if soft deleteion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(Guid id, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on ids.
    /// </summary>
    /// <param name="ids">The ids of the entities to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="isAtomic">If set to true then if one soft delete fails then all soft deletes will be reverted and the operation fails, otherwise the operation will succeed even if not all entities were succesfully soft deleted.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if <paramref name="isAtomic"/> is set to true and one soft delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(IEnumerable<Guid> ids, CancellationToken ct, bool isAtomic = true);

    /// <summary>
    /// Soft delete multiple entities from the database based on a given filter.
    /// </summary>
    /// <param name="filter">The filter to apply to search for entities to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="isAtomic">If set to true then if one soft delete fails then all soft deletes will be reverted and the operation fails, otherwise the operation will succeed even if not all entities were succesfully soft deleted.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if <paramref name="isAtomic"/> is set to true and one soft delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(Func<T, bool> filter, CancellationToken ct, bool isAtomic = true);

    /// <summary>
    /// Hard deletes a single entity from the database based on id.
    /// </summary>
    /// <param name="id">The id of the entity to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.DeleteOperationFailedError"/> if hard deleteion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(Guid id, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on ids.
    /// </summary>
    /// <param name="ids">The ids of the entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="isAtomic">If set to true then if one hard delete fails then all hard deletes will be reverted and the operation fails, otherwise the operation will succeed even if not all entities were succesfully hard deleted.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if <paramref name="isAtomic"/> is set to true and one hard delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(IEnumerable<Guid> ids, CancellationToken ct, bool isAtomic = true);

    /// <summary>
    /// Hard delete multiple entities from the database based on a given filter.
    /// </summary>
    /// <param name="filter">The filter to apply to search for entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="isAtomic">If set to true then if one hard delete fails then all hard deletes will be reverted and the operation fails, otherwise the operation will succeed even if not all entities were succesfully hard deleted.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if <paramref name="isAtomic"/> is set to true and one hard delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(Func<T, bool> filter, CancellationToken ct, bool isAtomic = true);
}
