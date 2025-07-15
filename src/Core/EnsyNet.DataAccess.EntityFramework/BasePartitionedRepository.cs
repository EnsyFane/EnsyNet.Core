using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;
using EnsyNet.DataAccess.EntityFramework.Extensions;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Linq.Expressions;

namespace EnsyNet.DataAccess.EntityFramework;

[PublicAPI]
public abstract class BasePartitionedRepository<T> : BaseRepository<T>, IPartitionedRepository<T> where T : PartitionedDbEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasePartitionedRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> to be used by the repository.</param>
    /// <param name="dbSet">The entity set that the repository will operate on.</param>
    /// <param name="logger">Logger instance to be used by the repository to log errors and warnings.</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected BasePartitionedRepository(DbContext dbContext, DbSet<T> dbSet, ILogger logger) : base(dbContext, dbSet, logger) { }

    /// <inheritdoc />
    public Task<Result<T>> GetById(Guid orgId, Guid id, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var entity = await DbSet
                .Where(x => x.OrgId == orgId 
                    && x.Id == id)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                Logger.LogWarning("Entity of type {EntityType} with id {EntityId} was not found.", typeof(T).Name, id);
                return Result.FromError<T>(new EntityNotFoundError<T>());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public Task<Result<T>> GetByExpression(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var entity = await DbSet
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                Logger.LogWarning("Entity of type {EntityType} was not found.", typeof(T).Name);
                return Result.FromError<T>(new EntityNotFoundError<T>());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetAll(Guid orgId, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var entities = await DbSet
                .Where(x => x.OrgId == orgId)
                .ToListAsync(ct);
            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetAll(Guid orgId, SortingQuery<T> sortingQuery, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var dbSet = DbSet.Where(x => x.OrgId == orgId);

            var entities = sortingQuery.IsAscending
                ? await dbSet.OrderBy(sortingQuery.SortFieldSelector).ToListAsync(ct)
                : await dbSet.OrderByDescending(sortingQuery.SortFieldSelector).ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetMany(Guid orgId, PaginationQuery paginationQuery, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var entities = await DbSet
                .Where(x => x.OrgId == orgId)
                .OrderBy(x => x.Id)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetMany(Guid orgId, PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var dbSet = DbSet.Where(x => x.OrgId == orgId);

            var orderedEntities = sortingQuery.IsAscending
                ? dbSet.OrderBy(sortingQuery.SortFieldSelector)
                : dbSet.OrderByDescending(sortingQuery.SortFieldSelector);

            var entities = await orderedEntities
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var entities = await DbSet
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var entities = await DbSet
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .OrderBy(x => x.Id)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, SortingQuery<T> sortingQuery, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var dbSet = DbSet.Where(x => x.OrgId == orgId);

            var orderedEntities = sortingQuery.IsAscending
                ? dbSet.OrderBy(sortingQuery.SortFieldSelector)
                : dbSet.OrderByDescending(sortingQuery.SortFieldSelector);

            var entities = await orderedEntities
                .Where(filter)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression(Guid orgId, Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var dbSet = DbSet.Where(x => x.OrgId == orgId);

            var orderedEntities = sortingQuery.IsAscending
                ? dbSet.OrderBy(sortingQuery.SortFieldSelector)
                : dbSet.OrderByDescending(sortingQuery.SortFieldSelector);

            var entities = await orderedEntities
                .Where(filter)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public Task<Result> Update(Guid orgId, Guid id, Expression<Func<EntityUpdates<T>, EntityUpdates<T>>> updateExpression, CancellationToken ct)
        => ExecuteSimpleDbQuery(async () =>
        {
            var transformedExpression = updateExpression.GetSetPropertyCallsExpression();
            var sanitizedUpdateExpression = SanitizeUpdateExpression(transformedExpression);

            var affectedRows = await DbSet
                .Where(x => x.OrgId == orgId 
                    && x.Id == id)
                .ExecuteUpdateAsync(sanitizedUpdateExpression, ct);

            if (affectedRows == 0)
            {
                Logger.LogError("Entity of type {EntityType} with id {EntityId} was not updated.", typeof(T).Name, id);
                return Result.FromError<int>(new UpdateOperationFailedError());
            }

            return Result.Ok(1);
        });

    /// <inheritdoc />
    public Task<Result<int>> Update(Guid orgId, IDictionary<Guid, Expression<Func<EntityUpdates<T>, EntityUpdates<T>>>> idToUpdateMap, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var totalAffectedRows = 0;
            foreach (var kvp in idToUpdateMap)
            {
                var transformedExpression = kvp.Value.GetSetPropertyCallsExpression();
                var sanitizedUpdateExpression = SanitizeUpdateExpression(transformedExpression);

                var affectedRows = await DbSet
                    .Where(x => x.OrgId == orgId 
                        && x.Id == kvp.Key)
                    .ExecuteUpdateAsync(sanitizedUpdateExpression, ct);
                totalAffectedRows += affectedRows;
            }

            if (totalAffectedRows == 0)
            {
                Logger.LogError("Entities of type {EntityType} were not updated.", typeof(T).Name);
                return Result.FromError<int>(new BulkUpdateOperationFailedError());
            }
            
            if (totalAffectedRows != idToUpdateMap.Count)
            {
                Logger.LogWarning("Not all entities of type {EntityType} were updated.", typeof(T).Name);
            }

            return Result.Ok(totalAffectedRows);
        });

    /// <inheritdoc />
    public Task<Result<int>> UpdateAtomic(Guid orgId, IDictionary<Guid, Expression<Func<EntityUpdates<T>, EntityUpdates<T>>>> idToUpdateMap, CancellationToken ct)
        => ExecuteAtomicDbQuery(async () =>
        {
            var totalAffectedRows = 0;
            foreach (var kvp in idToUpdateMap)
            {
                var transformedExpression = kvp.Value.GetSetPropertyCallsExpression();
                var sanitizedUpdateExpression = SanitizeUpdateExpression(transformedExpression);

                var affectedRows = await DbSet
                    .Where(x => x.OrgId == orgId 
                        && x.Id == kvp.Key)
                    .ExecuteUpdateAsync(sanitizedUpdateExpression, ct);
                totalAffectedRows += affectedRows;
            }

            if (totalAffectedRows != idToUpdateMap.Count)
            {
                Logger.LogError("Entities of type {EntityType} were not updated.", typeof(T).Name);
                return Result.FromError<int>(new BulkUpdateOperationFailedError());
            }

            return Result.Ok(totalAffectedRows);
        }, ct);

    /// <inheritdoc />
    public Task<Result> SoftDelete(Guid orgId, Guid id, CancellationToken ct)
        => ExecuteSimpleDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .Where(x => x.OrgId == orgId 
                    && x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                Logger.LogError("Entity of type {EntityType} with id {EntityId} was not soft deleted.", typeof(T).Name, id);
                return Result.FromError<int>(new DeleteOperationFailedError());
            }

            return Result.Ok(1);
        });

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .Where(x => x.OrgId == orgId 
                    && ids.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotSoftDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            if (affectedRows != ids.Count())
            {
                Logger.LogWarning("Not all entities of type {EntityType} were soft deleted.", typeof(T).Name);
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotSoftDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public Task<Result<int>> SoftDeleteAtomic(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct)
        => ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .Where(x => x.OrgId == orgId 
                    && ids.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotSoftDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public Task<Result<int>> SoftDeleteAtomic(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct)
        => ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotSoftDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public Task<Result> HardDelete(Guid orgId, Guid id, CancellationToken ct)
        => ExecuteSimpleDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .IgnoreQueryFilters()
                .Where(x => x.OrgId == orgId 
                    && x.Id == id)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                Logger.LogError("Entity of type {EntityType} with id {EntityId} was not hard deleted.", typeof(T).Name, id);
                return Result.FromError<int>(new DeleteOperationFailedError());
            }

            return Result.Ok(1);
        });

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .IgnoreQueryFilters()
                .Where(x => x.OrgId == orgId 
                    && ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotHardDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            if (affectedRows != ids.Count())
            {
                Logger.LogWarning("Not all entities of type {EntityType} were hard deleted.", typeof(T).Name);
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct)
        => ExecuteDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .IgnoreQueryFilters()
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotHardDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public Task<Result<int>> HardDeleteAtomic(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct)
        => ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .IgnoreQueryFilters()
                .Where(x => x.OrgId == orgId 
                    && ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotHardDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public Task<Result<int>> HardDeleteAtomic(Guid orgId, Expression<Func<T, bool>> filter, CancellationToken ct)
        => ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await DbSet
                .IgnoreQueryFilters()
                .Where(x => x.OrgId == orgId)
                .Where(filter)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                Logger.LogError(EntitiesNotHardDeletedError, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);
}
