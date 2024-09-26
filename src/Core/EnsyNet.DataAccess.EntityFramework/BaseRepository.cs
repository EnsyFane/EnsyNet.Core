using EnsyNet.Core.Errors;
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

/// <summary>
/// Base repository class for CRUD operations.
/// </summary>
/// <typeparam name="T">The type of the entity that this repository operates on.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Needed until we know what exceptions can be thrown by EF.")]
public abstract partial class BaseRepository<T> : IRepository<T> where T : DbEntity
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> to be used by the repository.</param>
    /// <param name="dbSet">The entity set that the repository will operate on.</param>
    /// <param name="logger">Logger instance to be used by the repository to log errors and warnings.</param>
    /// <exception cref="ArgumentNullException"></exception>
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
                _logger.LogError(ENTITIES_NOT_SOFT_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_SOFT_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_SOFT_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_SOFT_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_HARD_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_HARD_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_HARD_DELETED_ERROR, typeof(T).Name);
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
                _logger.LogError(ENTITIES_NOT_HARD_DELETED_ERROR, typeof(T).Name);
                return Result.FromError<int>(new BulkDeleteOperationFailedError());
            }

            return Result.Ok(affectedRows);
        }, ct);

    /// <summary>
    /// Method that can be used to sanitize an entity before inserting it into the database when not using a predefined Insert method.
    /// </summary>
    /// <param name="entity">The entity to be sanitized.</param>
    /// <returns>A new <see cref="DbEntity"/> that has been sanitized, ready to be inserted in the database.</returns>
    protected static T SanitizeEntityForInsert(T entity)
        => entity with
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null,
        };

    /// <summary>
    /// Method that can be used to sanitize an update expression before updating an entity in the database when not using a predefined Update method.
    /// </summary>
    /// <param name="updateExpression">The expression to be sanitized.</param>
    /// <returns>A new expression that has been sanitized, ready to be used to update a <see cref="DbEntity"/>.</returns>
    protected static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> SanitizeUpdateExpression(Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression)
        => updateExpression.AddExpression(x => x.SetProperty(t => t.UpdatedAt, DateTime.UtcNow))
            .AddExpression(x => x.SetProperty(t => t.CreatedAt, t => t.CreatedAt))
            .AddExpression(x => x.SetProperty(t => t.DeletedAt, (DateTime?)null));

    /// <summary>
    /// Executes a database query and handles exceptions that can be thrown by Entity Framework.
    /// </summary>
    /// <typeparam name="TResult">The type of the data to be returned.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>The result of executing the query.</returns>
    protected async Task<Result<TResult>> ExecuteDbQuery<TResult>(Func<Task<Result<TResult>>> func)
    {
        try
        {
            return await func();
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning(e, "Operation was canceled while executing db query for entity of type {EntityType}.", typeof(T).Name);
            return Result.FromError<TResult>(new OperationCanceledError(e));
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Error while saving changes to the database for entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new UnexpectedDatabaseError(e));
        }
        catch (Exception e) when (InvalidUpdateExpressionExceptionMessageRegex().IsMatch(e.Message))
        {
            _logger.LogError(e, "Invalid update expression when updating entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new InvalidUpdateEntityExpressionError());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while executing db query for entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new UnexpectedDatabaseError(e));
        }
    }

    /// <summary>
    /// Executes a database query atomically and handles exceptions that can be thrown by Entity Framework.
    /// </summary>
    /// <remarks>If errors are returned by <paramref name="func"/> then the operation is rolled back.</remarks>
    /// <typeparam name="TResult">The type of the data to be returned.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="ct">The cancellation token to use for the operation.</param>
    /// <returns>The result of executing the query.</returns>
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
        catch (OperationCanceledException e)
        {
            _logger.LogWarning(e, "Operation was cancelled while executing atomic db query for entity of type {EntityType}.", typeof(T).Name);
            return Result.FromError<TResult>(new OperationCanceledError(e));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while executing atomic db query for entity of type {EntityType}. Exception: {Exception}.", typeof(T).Name, e);
            return Result.FromError<TResult>(new UnexpectedDatabaseError(e));
        }
    }

    private const string ENTITIES_NOT_SOFT_DELETED_ERROR = "Entities of type {EntityType} were not soft deleted.";
    private const string ENTITIES_NOT_HARD_DELETED_ERROR = "Entities of type {EntityType} were not hard deleted.";

    [GeneratedRegex(@"The column name \'.*\' is specified more than once in .*")]
    private static partial Regex InvalidUpdateExpressionExceptionMessageRegex();
}
