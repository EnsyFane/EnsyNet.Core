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
    private readonly DbSet<T> _dbSet;
    private readonly ILogger _logger;

    protected BaseRepository(DbSet<T> dbSet, ILogger logger)
    {
        _dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetAll(CancellationToken ct)
    {
        try
        {
            var entities = await _dbSet.ToListAsync(ct);
            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting all entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetAll<TKey>(SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
    {
        try
        {
            var entities = sortingQuery.IsAscending
                ? await _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x)).ToListAsync(ct)
                : await _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x)).ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting all sorted entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<T>> GetByExpression(Func<T, bool> filter, CancellationToken ct)
    {
        try
        {
            var entity = await _dbSet.Where(x => filter(x)).FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                return Result.FromError<T>(new EntityNotFoundError());
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting all entities of type {EntityType} using expression. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<T>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<T>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var entity = await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync(ct);

            if (entity is null)
            {
                return Result.FromError<T>(new EntityNotFoundError());
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting entity of type {EntityType} with Id: {Id}. Exception: {Exception}.", typeof(T).Name, id, ex);
            return Result.FromError<T>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetMany(PaginationQuery paginationQuery, CancellationToken ct)
    {
        try
        {
            var entities = await _dbSet
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting paginated entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetMany<TKey>(PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
    {
        try
        {
            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x))
                : _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x));

            var entities = await orderedEntities
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting sorted and paginated entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, CancellationToken ct)
    {
        try
        {
            var entities = await _dbSet
                .Where(x => filter(x))
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting filtered entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, PaginationQuery paginationQuery, CancellationToken ct)
    {
        try
        {
            var entities = await _dbSet
                .Where(x => filter(x))
                .Skip(paginationQuery.Skip)
                .Take(paginationQuery.Take)
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting paginated and filtered entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
    {
        try
        {
            var orderedEntities = sortingQuery.IsAscending
                ? _dbSet.OrderBy(x => sortingQuery.SortFieldSelector(x))
                : _dbSet.OrderByDescending(x => sortingQuery.SortFieldSelector(x));

            var entities = await orderedEntities
                .Where(x => filter(x))
                .ToListAsync(ct);

            return Result.Ok<IEnumerable<T>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting sorted and filtered entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery, CancellationToken ct)
    {
        try
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
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting paginated, sorted and filtered entities of type {EntityType}. Exception: {Exception}.", typeof(T).Name, ex);
            return Result.FromError<IEnumerable<T>>(new UnexpectedDatabaseError(ex));
        }
    }

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(IEnumerable<Guid> ids, CancellationToken ct, bool isAtomic = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(Func<T, bool> filter, CancellationToken ct, bool isAtomic = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<T>> Insert(T entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, CancellationToken ct, bool isAtomic = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(IEnumerable<Guid> ids, CancellationToken ct, bool isAtomic = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(Func<T, bool> filter, CancellationToken ct, bool isAtomic = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> Update(T entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> Update(IEnumerable<T> entities, CancellationToken ct, bool isAtomic = true)
    {
        throw new NotImplementedException();
    }
}
