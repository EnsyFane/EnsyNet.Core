using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using Microsoft.Extensions.Logging.Abstractions;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public sealed class TestRepository : BaseRepository<TestEntity>
{
    public TestRepository(TestDbContext dbContext) : base(dbContext, dbContext.TestEntities, NullLogger.Instance)
    {
    }
}
