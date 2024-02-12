using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;

namespace EnsyNet.DataAccess.EntityFramework;

public abstract class BaseRepository<T> : IRepository<T> where T : DbEntity
{
    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetAll<TKey>(SortingQuery<T, TKey> sortingQuery)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<T>> GetByExpression(Func<T, bool> filter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<T>> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetMany(PaginationQuery paginationQuery)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetMany<TKey>(PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression(Func<T, bool> filter, PaginationQuery paginationQuery)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, SortingQuery<T, TKey> sortingQuery)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> GetManyByExpression<TKey>(Func<T, bool> filter, PaginationQuery paginationQuery, SortingQuery<T, TKey> sortingQuery)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(IEnumerable<Guid> ids, bool revertOnFailure = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> HardDelete(Func<T, bool> filter, bool revertOnFailure = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<T>> Insert(T entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<T>>> Insert(IEnumerable<T> entities, bool revertOnFailure = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(IEnumerable<Guid> ids, bool revertOnFailure = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> SoftDelete(Func<T, bool> filter, bool revertOnFailure = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> Update(T entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result<int>> Update(IEnumerable<T> entities, bool revertOnFailure = true)
    {
        throw new NotImplementedException();
    }
}
