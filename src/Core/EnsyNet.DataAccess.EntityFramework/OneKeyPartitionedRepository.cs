using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnsyNet.DataAccess.EntityFramework;

public abstract class OneKeyPartitionedRepository<T> : BaseRepository<T>, IOneKeyPartitionedRepository<T>
    where T : DbEntity
{
    protected OneKeyPartitionedRepository(DbContext dbContext, DbSet<T> dbSet, ILogger logger) : base(dbContext, dbSet, logger) {}

    public Task<Result<T>> GetById(object partition, Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
