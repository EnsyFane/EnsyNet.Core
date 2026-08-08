using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using Microsoft.Extensions.Logging.Abstractions;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public sealed class ChildTestRepository : BaseRepository<ChildTestEntity>
{
    public ChildTestRepository(TestDbContext dbContext) : base(dbContext, dbContext.ChildTestEntities, NullLogger.Instance) { }
}
