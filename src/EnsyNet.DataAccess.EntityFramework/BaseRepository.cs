using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;
using EnsyNet.DataAccess.EntityFramework.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EnsyNet.DataAccess.EntityFramework;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Needed until we know what exceptions can be thrown by EF.")]
public abstract partial class BaseRepository<T> : IRepository<T> where T : DbEntity
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
                return Result.FromError<T>(new EntityNotFoundError<T>());
            }

            return Result.Ok(entity);
        });

    /// <inheritdoc />
    public async Task<Result<T>> GetByExpression(Expression<Func<T, bool>> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entity = await _dbSet
                .Where(filter)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                _logger.LogWarning("Entity of type {EntityType} was not found.", typeof(T).Name);
                return Result.FromError<T>(new EntityNotFoundError<T>());
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
    public async Task<Result<IEnumerable<T>>> GetAll(SortingQuery<T> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = sortingQuery.IsAscending
                ? await _dbSet.OrderBy(sortingQuery.SortFieldSelector).ToListAsync(ct)
                : await _dbSet.OrderByDescending(sortingQuery.SortFieldSelector).ToListAsync(ct);

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
    public async Task<Result<IEnumerable<T>>> GetMany(PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {

            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(sortingQuery.SortFieldSelector)
                : _dbSet.OrderByDescending(sortingQuery.SortFieldSelector);

            var entities = await orderedEntities
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = await _dbSet
                .Where(filter)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var entities = await _dbSet
                .Where(filter)
                .OrderBy(x => x.Id)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, SortingQuery<T> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(sortingQuery.SortFieldSelector)
                : _dbSet.OrderByDescending(sortingQuery.SortFieldSelector);

            var entities = await orderedEntities
                .Where(filter)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Expression<Func<T, bool>> filter, PaginationQuery paginationQuery, SortingQuery<T> sortingQuery, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(sortingQuery.SortFieldSelector)
                : _dbSet.OrderByDescending(sortingQuery.SortFieldSelector);

            var entities = await orderedEntities
                .Where(filter)
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        });

    /// <inheritdoc />
    public async Task<Result<T>> Insert(T entity, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var sanitizedEntity = SanitizeEntityForInsert(entity);

            await _dbSet.AddAsync(sanitizedEntity, ct);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entity of type {EntityType} was not inserted.", typeof(T).Name);
                return Result.FromError<T>(new InsertOperationFailedError());
            }

            return Result.Ok(sanitizedEntity);
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var insertedEntities = new List<T>();
            foreach(var entity in entities)
            {
                var sanitized = SanitizeEntityForInsert(entity);
                await _dbSet.AddAsync(sanitized, ct);
                insertedEntities.Add(sanitized);
            }

            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not inserted.", typeof(T).Name);
                return Result.FromError<IEnumerable<T>>(new BulkInsertOperationFailedError());
            }
            else if (affectedRows != insertedEntities.Count)
            {
                _logger.LogWarning("Not all entities of type {EntityType} were inserted.", typeof(T).Name);
            }

            return Result.Ok(insertedEntities.AsEnumerable());
        });

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> InsertAtomic(IEnumerable<T> entities, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var insertedEntities = new List<T>();
            foreach (var entity in entities)
            {
                var sanitized = SanitizeEntityForInsert(entity);
                await _dbSet.AddAsync(sanitized, ct);
                insertedEntities.Add(sanitized);
            }

            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not inserted.", typeof(T).Name);
                return Result.FromError<IEnumerable<T>>(new BulkInsertOperationFailedError());
            }

            return Result.Ok(insertedEntities.AsEnumerable());
        }, ct);

    /// <inheritdoc />
    public async Task<Result> Update(Guid id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var sanitizedUpdateExpression = SanitizeUpdateExpression(updateExpression);

            var affectedRows = await _dbSet
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(sanitizedUpdateExpression, ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entity of type {EntityType} with id {EntityId} was not updated.", typeof(T).Name, id);
                return Result.FromError<int>(new UpdateOperationFailedError());
            }

            return Result.Ok(1);
        });

    /// <inheritdoc />
    public async Task<Result<int>> Update(IDictionary<Guid, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>> idToUpdateMap, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var totalAffectedRows = 0;
            foreach(var kvp in idToUpdateMap)
            {
                var sanitizedUpdateExpression = SanitizeUpdateExpression(kvp.Value);

                var affectedRows = await _dbSet
                    .Where(x => x.Id == kvp.Key)
                    .ExecuteUpdateAsync(sanitizedUpdateExpression, ct);
                totalAffectedRows += affectedRows;
            }

            if (totalAffectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not updated.", typeof(T).Name);
                return Result.FromError<int>(new BulkUpdateOperationFailedError());
            }
            else if (totalAffectedRows != idToUpdateMap.Count)
            {
                _logger.LogWarning("Not all entities of type {EntityType} were updated.", typeof(T).Name);
            }

            return Result.Ok(totalAffectedRows);
        });

    /// <inheritdoc />
    public async Task<Result<int>> UpdateAtomic(IDictionary<Guid, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>> idToUpdateMap, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var totalAffectedRows = 0;
            foreach (var kvp in idToUpdateMap)
            {
                var sanitizedUpdateExpression = SanitizeUpdateExpression(kvp.Value);

                var affectedRows = await _dbSet
                    .Where(x => x.Id == kvp.Key)
                    .ExecuteUpdateAsync(sanitizedUpdateExpression, ct);
                totalAffectedRows += affectedRows;
            }

            if (totalAffectedRows != idToUpdateMap.Count)
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
    public async Task<Result<int>> SoftDelete(Expression<Func<T, bool>> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(filter)
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
        => await ExecuteAtomicDbQuery(async () =>
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
    public async Task<Result<int>> SoftDeleteAtomic(Expression<Func<T, bool>> filter, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .Where(filter)
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
                .IgnoreQueryFilters()
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
                .IgnoreQueryFilters()
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
    public async Task<Result<int>> HardDelete(Expression<Func<T, bool>> filter, CancellationToken ct)
        => await ExecuteDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .IgnoreQueryFilters()
                .Where(filter)
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
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .IgnoreQueryFilters()
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
    public async Task<Result<int>> HardDeleteAtomic(Expression<Func<T, bool>> filter, CancellationToken ct)
        => await ExecuteAtomicDbQuery(async () =>
        {
            var affectedRows = await _dbSet
                .IgnoreQueryFilters()
                .Where(filter)
                .ExecuteDeleteAsync(ct);

            if (affectedRows == 0)
            {
                _logger.LogError("Entities of type {EntityType} were not hard deleted.", typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    protected T SanitizeEntityForInsert(T entity)
        => entity with
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null,
        };

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
        catch (Exception e) when (InvalidUpdateExpressionExceptionMessageRegex().IsMatch(e.Message))
        {
            _logger.LogError("Invalid update expression when updating entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new InvalidUpdateEntityExpressionError());
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
                return Result.FromError<TResult>(result.Error!);
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

    protected Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> SanitizeUpdateExpression(Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression)
        => updateExpression.AddExpression(x => x.SetProperty(t => t.UpdatedAt, DateTime.UtcNow))
            .AddExpression(x => x.SetProperty(t => t.CreatedAt, t => t.CreatedAt))
            .AddExpression(x => x.SetProperty(t => t.DeletedAt, (DateTime?)null));

    [GeneratedRegex(@"The column name \'.*\' is specified more than once in .*")]
    private static partial Regex InvalidUpdateExpressionExceptionMessageRegex();
}
