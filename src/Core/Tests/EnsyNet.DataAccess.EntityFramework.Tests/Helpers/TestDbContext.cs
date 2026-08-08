using EnsyNet.DataAccess.EntityFramework.Tests.Models;
using EnsyNet.DataAccess.EntityFramework.Configuration;

using Microsoft.EntityFrameworkCore;

namespace EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

public sealed class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public DbSet<ChildTestEntity> ChildTestEntities { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(builder =>
        {
            builder.ConfigureBaseProperties();
            builder.HasIndex(e => e.UniqueField).IsUnique();
        });

        modelBuilder.Entity<ChildTestEntity>(builder =>
        {
            builder.ConfigureBaseProperties();
            builder.HasOne<TestEntity>()
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
