﻿using FluentAssertions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class InsertTests : RepositoryTestsBase
{
    [Fact]
    public async Task InsertEntity_EntityInserted()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);

        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        AssertEntity(entity, ValidEntity);
    }

    [Fact]
    public async Task InsertEntityWithGeneratedFields_EntityFieldsOverwrittern()
    {
        var time = DateTime.UtcNow.AddDays(-1);
        var entityToInsert = ValidEntity with
        {
            CreatedAt = time,
            UpdatedAt = time,
            DeletedAt = time,
        };

        var insertResult = await Repository.Insert(entityToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task InsertEntityWithId_EntityIdOverwritten()
    {
        var entityToInsert = ValidEntity with
        {
            Id = Guid.NewGuid(),
        };

        var insertResult = await Repository.Insert(entityToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        entity.Id.Should().NotBe(entityToInsert.Id);
    }

    [Fact]
    public async Task InsertMultipleEnties_EntitiesInserted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            AssertEntity(entity, ValidEntity);
        }
    }

    [Fact]
    public async Task InsertEntitiesWithGeneratedFields_EntitiesFieldsOverwrittern()
    {
        var time = DateTime.UtcNow.AddDays(-1);
        var entityToInsert = ValidEntity with
        {
            CreatedAt = time,
            UpdatedAt = time,
            DeletedAt = time,
        };

        var insertResult = await Repository.Insert([entityToInsert, entityToInsert], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.DeletedAt.Should().BeNull();
        }
    }

    [Fact]
    public async Task InsertEntitiesWithId_EntitiesIdOverwritten()
    {
        var entitiesToInsert = new[] { ValidEntity with { Id = Guid.NewGuid() }, ValidEntity with { Id = Guid.NewGuid() } };

        var insertResult = await Repository.Insert(entitiesToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        entities.First().Id.Should().NotBe(entitiesToInsert[0].Id);
        entities.Last().Id.Should().NotBe(entitiesToInsert[^1].Id);
    }

    [Fact]
    public async Task AtomicInsertMultipleEnties_EntitiesInserted()
    {
        var insertResult = await Repository.InsertAtomic([ValidEntity, ValidEntity], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            AssertEntity(entity, ValidEntity);
        }
    }

    [Fact]
    public async Task AtomicInsertEntitiesWithGeneratedFields_EntitiesFieldsOverwrittern()
    {
        var time = DateTime.UtcNow.AddDays(-1);
        var entityToInsert = ValidEntity with
        {
            CreatedAt = time,
            UpdatedAt = time,
            DeletedAt = time,
        };

        var insertResult = await Repository.InsertAtomic([entityToInsert, entityToInsert], default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        foreach (var entity in entities)
        {
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.DeletedAt.Should().BeNull();
        }
    }

    [Fact]
    public async Task AtomicInsertEntitiesWithId_EntitiesIdOverwritten()
    {
        var entitiesToInsert = new[] { ValidEntity with { Id = Guid.NewGuid() }, ValidEntity with { Id = Guid.NewGuid() } };

        var insertResult = await Repository.InsertAtomic(entitiesToInsert, default);

        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        entities.First().Id.Should().NotBe(entitiesToInsert[0].Id);
        entities.Last().Id.Should().NotBe(entitiesToInsert[^1].Id);
    }
}
