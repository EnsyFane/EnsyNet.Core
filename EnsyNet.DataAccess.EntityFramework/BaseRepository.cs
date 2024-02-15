﻿using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnsyNet.DataAccess.EntityFramework;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Needed until we know what exceptions can be thrown by EF.")]
public abstract class BaseRepository<T> : IRepository<T> where T : DbEntity
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger _logger;

    protected BaseRepository(DbContext dbContext, DbSet<T> dbSet, ILogger logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result<T>> GetById(Guid id, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entity = await _dbSet
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                _logger.LogWarning("Entity of type {EntityType} with id {EntityId} was not found.", typeof(T).Name, id);
                return Result.FromError<T>(new EntityNotFoundError());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<T>> GetByExpression(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entity = await _dbSet
                .Where(x => filter(x))
                .FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                _logger.LogWarning("Entity of type {EntityType} was not found.", typeof(T).Name);
                return Result.FromError<T>(new EntityNotFoundError());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetAll(CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = await _dbSet.ToListAsync(ct);
            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetAll<TKey>(SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = sortingQuery.IsAscending
                ? await _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x)).ToListAsync(ct)
                : await _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x)).ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetMany(PaginationQuery paginationQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = await _dbSet
                .OrderBy(x => x.Id)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetMany<TKey>(PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {

            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x))
                : _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x));

            var entities = await orderedEntities
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = await _dbSet
                .Where(x => filter(x))
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, PaginationQuery paginationQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = await _dbSet
                .Where(x => filter(x))
                .OrderBy(x => x.Id)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x))
                : _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x));

            var entities = await orderedEntities
                .Where(x => filter(x))
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x))
                : _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x));

            var entities = await orderedEntities
                .Where(x => filter(x))
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<T>> Insert(T entity, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            await _dbSet.AddAsync(entity, ct);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entity of type {EntityType} was not inserted.", typeof(T).Name);
                return Result.FromError<T>(new InsertOperationFailedError());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            await _dbSet.AddRangeAsync(entities, ct);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not inserted.", typeof(T).Name);
                return Result.FromError<IEnumerable<T>>(new BulkInsertOperationFailedError());
            }
            else if (affectedRows != entities.Count())
            {
                _logger.LogWarning("Not all entities of type {EntityType} were inserted.", typeof(T).Name);
            }

            return Result.Ok(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> InsertAtomic(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteAtomicDbQuery<IEnumerable<T>, BulkInsertOperationFailedError>(async () =>
        {
            await _dbSet.AddRangeAsync(entities, ct);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not inserted.", typeof(T).Name);
                return Result.FromError<IEnumerable<T>>(new BulkInsertOperationFailedError());
            }

            return Result.Ok(entities);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<T>> Update(T entity, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => x.Id == entity.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t, entity) , ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entity of type {EntityType} with id {EntityId} was not updated.", typeof(T).Name, entity.Id);
                return Result.FromError<T>(new UpdateOperationFailedError());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<int>> Update(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var totalAffectedRows = 0;
            foreach(var entity in entities)
            {
                var affectedRows = await _dbSet
                    .Where(x => x.Id == entity.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(t => t, entity), ct);
                totalAffectedRows += affectedRows;
            }

            if (totalAffectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not updated.", typeof(T).Name);
                return Result.FromError<int>(new BulkUpdateOperationFailedError());
            }
            else if (totalAffectedRows != entities.Count())
            {
                _logger.LogWarning("Not all entities of type {EntityType} were updated.", typeof(T).Name);
            }

            return Result.Ok(totalAffectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> UpdateAtomic(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteAtomicDbQuery<int, BulkUpdateOperationFailedError>(async () =>
        {
            var totalAffectedRows = 0;
            foreach (var entity in entities)
            {
                var affectedRows = await _dbSet
                    .Where(x => x.Id == entity.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(t => t, entity), ct);
                totalAffectedRows += affectedRows;
            }

            if (totalAffectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not updated.", typeof(T).Name);
                return Result.FromError<int>(new BulkUpdateOperationFailedError());
            }

            return Result.Ok(totalAffectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result> SoftDelete(Guid id, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entity of type {EntityType} with id {EntityId} was not soft deleted.", typeof(T).Name, id);
                return Result.FromError<int>(new DeleteOperationFailedError());
            }

            return Result.Ok(1);
        });

    /// <inheritdoc />
    public async Task<Result<int>> SoftDelete(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not soft deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }
            else if (affectedRows != ids.Count())
            {
                _logger.LogWarning("Not all entities of type {EntityType} were soft deleted.", typeof(T).Name);
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> SoftDelete(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not soft deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> SoftDeleteAtomic(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteAtomicDbQuery<int, BulkDeleteOperationFailedError>(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not soft deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<int>> SoftDeleteAtomic(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteAtomicDbQuery<int, BulkDeleteOperationFailedError>(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not soft deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result> HardDelete(Guid id, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entity of type {EntityType} with id {EntityId} was not hard deleted.", typeof(T).Name, id);
                return Result.FromError<int>(new DeleteOperationFailedError());
            }

            return Result.Ok(1);
        });

    /// <inheritdoc />
    public async Task<Result<int>> HardDelete(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not hard deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }
            else if (affectedRows != ids.Count())
            {
                _logger.LogWarning("Not all entities of type {EntityType} were hard deleted.", typeof(T).Name);
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> HardDelete(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not hard deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> HardDeleteAtomic(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteAtomicDbQuery<int, BulkDeleteOperationFailedError>(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not hard deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<int>> HardDeleteAtomic(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteAtomicDbQuery<int, BulkDeleteOperationFailedError>(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not hard deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    protected async Task<Result<TResult>> ExecuteDbQuery<TResult>(Func<Task<Result<TResult>>> func)
    {
        try
        {
            return await func();
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning(e, "Operation was cancelled while executing db query for entity of type {EntityType}.", typeof(T).Name);
            throw;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError("Error while saving changes to the database for entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new UnexpectedDatabaseError(e));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while executing db query for entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new UnexpectedDatabaseError(e));
        }
    }

    protected async Task<Result<TResult>> ExecuteAtomicDbQuery<TResult, TError>(Func<Task<Result<TResult>>> func, CancellationToken ct) where TError : Error, new()
    {
        try
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            var result = await ExecuteDbQuery(func);
            if (result.HasError)
            {
                await transaction.RollbackAsync(ct);
                return Result.FromError<TResult>(new TError());
            }
            await transaction.CommitAsync(ct);

            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Operation was cancelled while executing atomic db query for entity of type {EntityType}.", typeof(T).Name);
            throw;
        }
    }
}
