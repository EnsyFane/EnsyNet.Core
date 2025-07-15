using EnsyNet.DataAccess.EntityFramework.Tests.Models;
using EnsyNet.DataAccess.EntityFramework.Configuration;

using Microsoft.EntityFrameworkCore;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public sealed class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<PartitionedTestEntity> PartitionedTestEntities { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>().ConfigureBaseProperties();
        modelBuilder.Entity<PartitionedTestEntity>().ConfigurePartitionedBaseProperties();
    }
}
