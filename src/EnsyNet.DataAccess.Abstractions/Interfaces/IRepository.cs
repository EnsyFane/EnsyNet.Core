using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore.Query;

using System.Linq.Expressions;

namespace EnsyNet.DataAccess.Abstractions.Interfaces;

/// <summary>
/// Repository containing all supported operations that can be performed on a database entity.
/// </summary>
/// <typeparam name="T">The type of the entity stored in the database.</typeparam>
public interface IRepository<T> where T : DbEntity
{
    /// <summary>
    /// Retrieves a single entity from the database by its <see cref="DbEntity.Id"/>.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError{T}"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetById(Guid id, CancellationToken ct);

    /// <summary>
    /// Retrieves the first entity from the database that matches the given <paramref name="filter"/>.
    /// </summary>
    /// <returns>
    /// <param name="filter">The filter expression to be used for the database query.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError{T}"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetByExpression(Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Retrieves all entities from the database. Not recommended.
    /// </summary>
    /// <remarks>This might be a resource intensive operation and can lead to an <see cref="OutOfMemoryException"/>.</remarks>
    /// <returns>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetAll(CancellationToken ct);

    /// <summary>
    /// Retrieves all entities from the database sorted based on the provided <see cref="SortingQuery{T}"/>. Not recommended.
    /// </summary>
    /// <remarks>This might be a resource intensive operation and can lead to an <see cref="OutOfMemoryException"/>.</remarks>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetAll(SortingQuery<T> sortingQuery, CancellationToken ct);

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
    /// Retrieves entities from the database paginated based on the provided <see cref="PaginationQuery"/> and sorted based on the provided <see cref="SortingQuery{T}"/>. 
    /// </summary>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetMany(PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct);

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
    Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, CancellationToken ct);

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
    Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter and sorted based on the provided <see cref="SortingQuery{T}"/>. 
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, SortingQuery<T> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter, paginated based on the provided <see cref="PaginationQuery"/> 
    /// and sorted based on the provided <see cref="SortingQuery{T}"/>. 
    /// </summary>
    /// <param name="filter">The filter to use.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Inserts a single entity into the database.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/> and <see cref="DbEntity.CreatedAt"/> fields will be overwritten and the <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields will be set to null.</remarks>
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
    /// If one insertion fails, the operation will continue and the inserted entities will be returned.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/> and <see cref="DbEntity.CreatedAt"/> fields will be overwritten and the <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields will be set to null.</remarks>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The inserted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, CancellationToken ct);

    /// <summary>
    /// Inserts multiple entities into the database. Will fail and revert if one insertion fails.
    /// If one insertion fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/> and <see cref="DbEntity.CreatedAt"/> fields will be overwritten and the <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields will be set to null.</remarks>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The inserted entities or <br/>
    /// An <see cref="Errors.BulkInsertOperationFailedError"/> if one insertion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> InsertAtomic(IEnumerable<T> entities, CancellationToken ct);

    /// <summary>
    /// Updates a single entity from the database.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/>, <see cref="DbEntity.CreatedAt"/>, <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields can not be updated manually.</remarks>
    /// <param name="id">The id of the entity to update.</param>
    /// <param name="updateExpression">An expressionthat describes the updates that need to be aplied to the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Result"/> or <br/>
    /// An <see cref="Errors.InvalidUpdateEntityExpressionError"/> if the user provided an invalid update expression or <br/>
    /// An <see cref="Errors.UpdateOperationFailedError"/> if update fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result> Update(Guid id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression, CancellationToken ct);

    /// <summary>
    /// Updates multiple entities from the database.
    /// If one update fails, the operation will continue and the updated entities will be returned.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/>, <see cref="DbEntity.CreatedAt"/>, <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields can not be update manually.</remarks>
    /// <param name="idToUpdateMap">A map from an entity id to an expression that describes the updates that need to be aplied to the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of updated entities or <br/>
    /// An <see cref="Errors.InvalidUpdateEntityExpressionError"/> if the user provided an invalid update expression or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> Update(IDictionary<Guid, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>> idToUpdateMap, CancellationToken ct);

    /// <summary>
    /// Updates multiple entities from the database. Will fail and rollback if one update fails.
    /// If one update fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <remarks>The <see cref="DbEntity.Id"/>, <see cref="DbEntity.CreatedAt"/>, <see cref="DbEntity.UpdatedAt"/> and <see cref="DbEntity.DeletedAt"/> fields can not be update manually.</remarks>
    /// <param name="idToUpdateMap">A map from an entity id to an expression that describes the updates that need to be aplied to the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of updated entities or <br/>
    /// An <see cref="Errors.InvalidUpdateEntityExpressionError"/> if the user provided an invalid update expression or <br/>
    /// An <see cref="Errors.BulkUpdateOperationFailedError"/> if one update fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> UpdateAtomic(IDictionary<Guid, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>> idToUpdateMap, CancellationToken ct);

    /// <summary>
    /// Soft deletes a single entity from the database based on id.
    /// </summary>
    /// <param name="id">The id of the entity to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Result"/> or <br/>
    /// An <see cref="Errors.DeleteOperationFailedError"/> if soft deleteion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result> SoftDelete(Guid id, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on ids.
    /// If one soft delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="ids">The ids of the entities to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on a given filter.
    /// If one soft delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="filter">The filter to apply to search for entities to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on ids. Will fail and rollback if one soft delete fails.
    /// If one soft delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="ids">The ids of the entities to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one soft delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDeleteAtomic(IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on a given filter. Will fail and rollback if one soft delete fails.
    /// If one soft delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="filter">The filter to apply to search for entities to soft delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one soft delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDeleteAtomic(Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Hard deletes a single entity from the database based on id.
    /// </summary>
    /// <param name="id">The id of the entity to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Result"/> or <br/>
    /// An <see cref="Errors.DeleteOperationFailedError"/> if hard deleteion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result> HardDelete(Guid id, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on ids.
    /// If one hard delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="ids">The ids of the entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on a given filter.
    /// If one hard delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="filter">The filter to apply to search for entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on ids. Will fail and rollback if one hard delete fails.
    /// If one hard delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="ids">The ids of the entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one hard delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDeleteAtomic(IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on a given filter. Will fail and rollback if one hard delete fails.
    /// If one hard delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="filter">The filter to apply to search for entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one hard delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDeleteAtomic(Expression<Func<T, bool>> filter, CancellationToken ct);
}
