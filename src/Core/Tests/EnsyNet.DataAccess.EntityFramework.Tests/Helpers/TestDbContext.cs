using EnsyNet.DataAccess.EntityFramework.Tests.Models;
using EnsyNet.DataAccess.EntityFramework.Configuration;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public sealed class TestDbContext : DbContext
{
    [PublicAPI]
    public DbSet<TestEntity> TestEntities { get; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>().ConfigureBaseProperties();
    }
}
