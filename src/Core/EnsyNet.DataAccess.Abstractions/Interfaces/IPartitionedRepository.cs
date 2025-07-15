using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Models;

using System.Linq.Expressions;

using JetBrains.Annotations;

namespace EnsyNet.DataAccess.Abstractions.Interfaces;

/// <summary>
/// Partitioned repository containing all supported operations that can be performed on a database entity.
/// </summary>
/// <remarks>OrgId is the partition key for the default entity. It needs to be supplied in each call.</remarks>
/// <typeparam name="T">The type of the entity stored in the database.</typeparam>
[PublicAPI]
public interface IPartitionedRepository<T> where T : PartitionedDbEntity
{
    /// <summary>
    /// Retrieves a single entity from the database by its <see cref="PartitionedDbEntity.Id"/>.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="id">The id of the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError{T}"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetById(Guid orgId, Guid id, CancellationToken ct);

    /// <summary>
    /// Retrieves the first entity from the database that matches the given <paramref name="filter"/>.
    /// </summary>
    /// <returns>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter expression to be used for the database query.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// The found entity or <br/>
    /// An <see cref="Errors.EntityNotFoundError{T}"/> if no entity was found or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<T>> GetByExpression(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Retrieves all entities from the database. Not recommended.
    /// </summary>
    /// <remarks>This might be a resource intensive operation and can lead to an <see cref="OutOfMemoryException"/>.</remarks>
    /// <returns>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetAll(Guid orgId, CancellationToken ct);

    /// <summary>
    /// Retrieves all entities from the database sorted based on the provided <see cref="SortingQuery{T}"/>. Not recommended.
    /// </summary>
    /// <remarks>This might be a resource intensive operation and can lead to an <see cref="OutOfMemoryException"/>.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetAll(Guid orgId, SortingQuery<T> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database paginated based on the provided <see cref="PaginationQuery"/>. 
    /// </summary>
    /// <remarks>Entities will be sorted ascending by <see cref="PartitionedDbEntity.Id"/>.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetMany(Guid orgId, PaginationQuery paginationQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database paginated based on the provided <see cref="PaginationQuery"/> and sorted based on the provided <see cref="SortingQuery{T}"/>. 
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetMany(Guid orgId, PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter. 
    /// </summary>
    /// <remarks>Entities will be sorted ascending by <see cref="PartitionedDbEntity.Id"/>.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter and paginated based on the provided <see cref="PaginationQuery"/>. 
    /// </summary>
    /// <remarks>Entities will be sorted ascending by <see cref="PartitionedDbEntity.Id"/>.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to use.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter and sorted based on the provided <see cref="SortingQuery{T}"/>. 
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, SortingQuery<T> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Retrieves entities from the database based on the provided filter, paginated based on the provided <see cref="PaginationQuery"/> 
    /// and sorted based on the provided <see cref="SortingQuery{T}"/>. 
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to use.</param>
    /// <param name="paginationQuery">The pagination query to use.</param>
    /// <param name="sortingQuery">The sorting query to use.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The found entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct);

    /// <summary>
    /// Inserts a single entity into the database.
    /// </summary>
    /// <remarks>The <see cref="PartitionedDbEntity.Id"/> and <see cref="PartitionedDbEntity.CreatedAt"/> fields will be overwritten and the <see cref="PartitionedDbEntity.UpdatedAt"/> and <see cref="PartitionedDbEntity.DeletedAt"/> fields will be set to null.</remarks>
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
    /// <remarks>The <see cref="PartitionedDbEntity.Id"/> and <see cref="PartitionedDbEntity.CreatedAt"/> fields will be overwritten and the <see cref="PartitionedDbEntity.UpdatedAt"/> and <see cref="PartitionedDbEntity.DeletedAt"/> fields will be set to null.</remarks>
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
    /// <remarks>The <see cref="PartitionedDbEntity.Id"/> and <see cref="PartitionedDbEntity.CreatedAt"/> fields will be overwritten and the <see cref="PartitionedDbEntity.UpdatedAt"/> and <see cref="PartitionedDbEntity.DeletedAt"/> fields will be set to null.</remarks>
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
    /// <remarks>The <see cref="PartitionedDbEntity.Id"/>, <see cref="PartitionedDbEntity.CreatedAt"/>, <see cref="PartitionedDbEntity.UpdatedAt"/> and <see cref="PartitionedDbEntity.DeletedAt"/> fields can not be updated manually.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="id">The id of the entity to update.</param>
    /// <param name="updateExpression">An expression that describes the updates that need to be applied to the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Result"/> or <br/>
    /// An <see cref="Errors.InvalidUpdateEntityExpressionError"/> if the user provided an invalid update expression or <br/>
    /// An <see cref="Errors.UpdateOperationFailedError"/> if update fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result> Update(Guid orgId, Guid id, Expression<Func<EntityUpdates<T>, EntityUpdates<T>>> updateExpression, CancellationToken ct);

    /// <summary>
    /// Updates multiple entities from the database.
    /// If one update fails, the operation will continue and the updated entities will be returned.
    /// </summary>
    /// <remarks>The <see cref="PartitionedDbEntity.Id"/>, <see cref="PartitionedDbEntity.CreatedAt"/>, <see cref="PartitionedDbEntity.UpdatedAt"/> and <see cref="PartitionedDbEntity.DeletedAt"/> fields can not be updated manually.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="idToUpdateMap">A map from an entity id to an expression that describes the updates that need to be applied to the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of updated entities or <br/>
    /// An <see cref="Errors.InvalidUpdateEntityExpressionError"/> if the user provided an invalid update expression or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> Update(Guid orgId, IDictionary<Guid, Expression<Func<EntityUpdates<T>, EntityUpdates<T>>>> idToUpdateMap, CancellationToken ct);

    /// <summary>
    /// Updates multiple entities from the database. Will fail and rollback if one update fails.
    /// If one update fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <remarks>The <see cref="PartitionedDbEntity.Id"/>, <see cref="PartitionedDbEntity.CreatedAt"/>, <see cref="PartitionedDbEntity.UpdatedAt"/> and <see cref="PartitionedDbEntity.DeletedAt"/> fields can not be updated manually.</remarks>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="idToUpdateMap">A map from an entity id to an expression that describes the updates that need to be applied to the entity.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of updated entities or <br/>
    /// An <see cref="Errors.InvalidUpdateEntityExpressionError"/> if the user provided an invalid update expression or <br/>
    /// An <see cref="Errors.BulkUpdateOperationFailedError"/> if one update fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> UpdateAtomic(Guid orgId, IDictionary<Guid, Expression<Func<EntityUpdates<T>, EntityUpdates<T>>>> idToUpdateMap, CancellationToken ct);

    /// <summary>
    /// Soft deletes a single entity from the database based on id.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="id">The id of the entity to soft-delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Result"/> or <br/>
    /// An <see cref="Errors.DeleteOperationFailedError"/> if soft deletion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result> SoftDelete(Guid orgId, Guid id, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on ids.
    /// If one soft delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="ids">The ids of the entities to soft-delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on a given filter.
    /// If one soft delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to apply to search for entities to soft-delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDelete(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on ids. Will fail and rollback if one soft delete fails.
    /// If one soft delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="ids">The ids of the entities to soft-delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one soft delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDeleteAtomic(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Soft delete multiple entities from the database based on a given filter. Will fail and rollback if one soft delete fails.
    /// If one soft delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to apply to search for entities to soft-delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of soft deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one soft delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> SoftDeleteAtomic(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Hard deletes a single entity from the database based on id.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="id">The id of the entity to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A successful <see cref="Result"/> or <br/>
    /// An <see cref="Errors.DeleteOperationFailedError"/> if hard deletion fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result> HardDelete(Guid orgId, Guid id, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on ids.
    /// If one hard delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="ids">The ids of the entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on a given filter.
    /// If one hard delete fails, the operation will continue and the number of deleted entities will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to apply to search for entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDelete(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on ids. Will fail and rollback if one hard delete fails.
    /// If one hard delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="ids">The ids of the entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one hard delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDeleteAtomic(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct);

    /// <summary>
    /// Hard delete multiple entities from the database based on a given filter. Will fail and rollback if one hard delete fails.
    /// If one hard delete fails, the operation will fail and <see cref="Errors.BulkInsertOperationFailedError"/> will be returned.
    /// </summary>
    /// <param name="orgId">The org id of the entity.</param>
    /// <param name="filter">The filter to apply to search for entities to hard delete.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The number of hard deleted entities or <br/>
    /// An <see cref="Errors.BulkDeleteOperationFailedError"/> if one hard delete fails or <br/>
    /// An <see cref="Errors.UnexpectedDatabaseError"/> if there was an unexpected database error.
    /// </returns>
    Task<Result<int>> HardDeleteAtomic(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct);
}
