using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.BaseRepositoryTests;

public class InsertTests
{
    private static readonly TestEntity _validEntity = new()
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

    [Fact]
    public async Task InsertEntity_EntityInserted()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var insertResult = await repository.Insert(_validEntity, default);

        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        AssertEntity(entity, _validEntity);
    }

    [Fact]
    public async Task InsertEntityWithGeneratedFields_EntityFieldsOverwrittern()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var time = DateTime.UtcNow.AddDays(-1);
        var entityToInsert = _validEntity with
        {
            CreatedAt = time,
            UpdatedAt = time,
            DeletedAt = time,
        };

        var insertResult = await repository.Insert(entityToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task InsertEntityWithId_EntityIdOverwritten()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entityToInsert = _validEntity with
        {
            Id = Guid.NewGuid(),
        };

        var insertResult = await repository.Insert(entityToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        entity.Id.Should().NotBe(entityToInsert.Id);
    }

    [Fact]
    public async Task InsertMultipleEnties_EntitiesInserted()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var insertResult = await repository.Insert([_validEntity, _validEntity], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach(var entity in entities)
        {
            AssertEntity(entity, _validEntity);
        }
    }

    [Fact]
    public async Task InsertEntitiesWithGeneratedFields_EntitiesFieldsOverwrittern()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var time = DateTime.UtcNow.AddDays(-1);
        var entityToInsert = _validEntity with
        {
            CreatedAt = time,
            UpdatedAt = time,
            DeletedAt = time,
        };

        var insertResult = await repository.Insert([entityToInsert, entityToInsert], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.UpdatedAt.Should().BeNull();
            entity.DeletedAt.Should().BeNull();
        }
    }

    [Fact]
    public async Task InsertEntitiesWithId_EntitiesIdOverwritten()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entitiesToInsert = new[] { _validEntity with { Id = Guid.NewGuid() }, _validEntity with { Id = Guid.NewGuid() } };

        var insertResult = await repository.Insert(entitiesToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        entities.First().Id.Should().NotBe(entitiesToInsert[0].Id);
        entities.Last().Id.Should().NotBe(entitiesToInsert[^1].Id);
    }

    [Fact]
    public async Task AtomicInsertMultipleEnties_EntitiesInserted()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var insertResult = await repository.InsertAtomic([_validEntity, _validEntity], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            AssertEntity(entity, _validEntity);
        }
    }

    [Fact]
    public async Task AtomicInsertEntitiesWithGeneratedFields_EntitiesFieldsOverwrittern()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var time = DateTime.UtcNow.AddDays(-1);
        var entityToInsert = _validEntity with
        {
            CreatedAt = time,
            UpdatedAt = time,
            DeletedAt = time,
        };

        var insertResult = await repository.InsertAtomic([entityToInsert, entityToInsert], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.UpdatedAt.Should().BeNull();
            entity.DeletedAt.Should().BeNull();
        }
    }

    [Fact]
    public async Task AtomicInsertEntitiesWithId_EntitiesIdOverwritten()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entitiesToInsert = new[] { _validEntity with { Id = Guid.NewGuid() }, _validEntity with { Id = Guid.NewGuid() } };

        var insertResult = await repository.InsertAtomic(entitiesToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        entities.First().Id.Should().NotBe(entitiesToInsert[0].Id);
        entities.Last().Id.Should().NotBe(entitiesToInsert[^1].Id);
    }

    private static void AssertEntity(TestEntity actualEntity, TestEntity expectedEntity)
    {
        actualEntity.Id.Should().NotBe(Guid.Empty);
        actualEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        actualEntity.UpdatedAt.Should().BeNull();
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
}
