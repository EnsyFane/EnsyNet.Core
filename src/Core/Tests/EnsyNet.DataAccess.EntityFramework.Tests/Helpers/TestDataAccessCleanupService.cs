using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Interfaces;
using EnsyNet.DataAccess.Cleanup.Implementations;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using Microsoft.Extensions.Logging;

using System.Linq.Expressions;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

internal sealed class TestDataAccessCleanupService : BaseDataAccessCleanupService<TestEntity>
{
    public Expression<Func<TestEntity, bool>> EntityFilter { get; set; } = x => true;

    public Func<TestEntity, Result<bool>> CanDeleteEntity { get; set; } = x => Result.Ok(true);

    public TestDataAccessCleanupService(IRepository<TestEntity> repository, ILogger<BaseDataAccessCleanupService<TestEntity>> logger) : base(repository, logger) { }

    protected override Expression<Func<TestEntity, bool>> FilterEntitiesToHardDelete 
    {
        get => EntityFilter;
    }

    protected override Task<Result<bool>> ExecuteBeforeDeletion(TestEntity entity, CancellationToken ct) 
        => Task.FromResult(CanDeleteEntity(entity));
}
