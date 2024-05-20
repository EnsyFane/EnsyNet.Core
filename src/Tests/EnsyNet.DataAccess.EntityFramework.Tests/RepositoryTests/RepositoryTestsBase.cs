﻿using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public abstract class RepositoryTestsBase : IDisposable
{
    protected readonly TestDbContext DbContext;
    protected readonly TestRepository Repository;

    protected RepositoryTestsBase()
    {
        DbContext = new TestDbContext(DatabaseConfiguration.Options);
        Repository = new TestRepository(DbContext);
    }

    protected readonly TestEntity ValidEntity = new()
    {
        StringField = "Test",
        IntField = 1,
        BoolField = true,
        FloatField = 1.1f,
        DoubleField = 1.2d,
        DecimalField = 1.4m,
        DateTimeField = DateTime.UtcNow,
        TimeSpanField = TimeSpan.FromHours(1),
        GuidField = Guid.NewGuid(),
    };

    protected static void AssertEntity(TestEntity actualEntity, TestEntity expectedEntity)
    {
        actualEntity.Id.Should().NotBe(Guid.Empty);
        actualEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        actualEntity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        actualEntity.DeletedAt.Should().BeNull();
        actualEntity.StringField.Should().Be(expectedEntity.StringField);
        actualEntity.IntField.Should().Be(expectedEntity.IntField);
        actualEntity.BoolField.Should().Be(expectedEntity.BoolField);
        actualEntity.FloatField.Should().Be(expectedEntity.FloatField);
        actualEntity.DoubleField.Should().Be(expectedEntity.DoubleField);
        actualEntity.DecimalField.Should().Be(expectedEntity.DecimalField);
        actualEntity.DateTimeField.Should().Be(expectedEntity.DateTimeField);
        actualEntity.TimeSpanField.Should().Be(expectedEntity.TimeSpanField);
        actualEntity.GuidField.Should().Be(expectedEntity.GuidField);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext.Dispose();
        }
    }
}
