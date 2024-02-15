using EnsyNet.Core.Results;
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
            var entity = await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                return Result.FromError<T>(new EntityNotFoundError());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<T>> GetByExpression(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entity = await _dbSet.Where(x => filter(x)).FirstOrDefaultAsync(ct);

            if (entity is null)
            {
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
            await _dbContext.SaveChangesAsync(ct);

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            await _dbSet.AddRangeAsync(entities, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Result.Ok(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> InsertAtomic(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            await _dbSet.AddRangeAsync(entities, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Result.Ok(entities);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<bool>> Update(T entity, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => x.Id == entity.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t, entity) , ct);

            return Result.Ok(affectedRows == 1);
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
            return Result.Ok(totalAffectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> UpdateAtomic(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var totalAffectedRows = 0;
            foreach (var entity in entities)
            {
                var affectedRows = await _dbSet
                    .Where(x => x.Id == entity.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(t => t, entity), ct);
                totalAffectedRows += affectedRows;
            }
            return Result.Ok(totalAffectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<bool>> SoftDelete(Guid id, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            return Result.Ok(affectedRows == 1);
        });

    /// <inheritdoc />
    public async Task<Result<int>> SoftDelete(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> SoftDelete(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> SoftDeleteAtomic(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<int>> SoftDeleteAtomic(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.DeletedAt, DateTime.UtcNow), ct);

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<bool>> HardDelete(Guid id, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(ct);

            return Result.Ok(affectedRows == 1);
        });

    /// <inheritdoc />
    public async Task<Result<int>> HardDelete(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> HardDelete(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteDeleteAsync(ct);

            return Result.Ok(affectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> HardDeleteAtomic(IEnumerable<Guid> ids, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => ids.Contains(x.Id))
                .ExecuteDeleteAsync(ct);

            return Result.Ok(affectedRows);
        }, ct);

    /// <inheritdoc />
    public async Task<Result<int>> HardDeleteAtomic(Func<T, bool> filter, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(x => filter(x))
                .ExecuteDeleteAsync(ct);

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

    protected async Task<Result<TResult>> ExecuteAtomicDbQuery<TResult>(Func<Task<Result<TResult>>> func, CancellationToken ct)
    {
        try
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            var result = await ExecuteDbQuery(func);
            if (result.HasError)
            {
                await transaction.RollbackAsync(ct);
                return result;
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
