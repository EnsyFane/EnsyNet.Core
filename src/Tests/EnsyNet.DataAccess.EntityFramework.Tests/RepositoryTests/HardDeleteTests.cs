﻿using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;
using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class HardDeleteTests : RepositoryTestsBase
{
    [Fact]
    public async Task EntityInserted_HardDelete_EntityHardDeleted()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;

        var deleteResult = await Repository.HardDelete(entity.Id, default);
        
        deleteResult.HasError.Should().BeFalse();
        await AssertEntityHardDeleted(entity);
    }

    [Fact]
    public async Task SoftDeletedEntity_HardDelete_EntityHardDeleted()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entity.Id, default);
        softDeleteResult.HasError.Should().BeFalse();

        var hardDeleteResult = await Repository.HardDelete(entity.Id, default);
        
        hardDeleteResult.HasError.Should().BeFalse();
        await AssertEntityHardDeleted(entity);
    }

    [Fact]
    public async Task EntitiesInserted_HardDeleteAtomicByIds_EntityHardDeleted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var toDelete = entities.Select(x => x.Id).Take(2);
        var softDeleteResult = await Repository.SoftDelete(toDelete, default);
        softDeleteResult.HasError.Should().BeFalse();
        var toKeep = entities.Select(x => x.Id).Skip(2).Single();

        var deleteResult = await Repository.HardDeleteAtomic(toDelete, default);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (toKeep == entity.Id)
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, default);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
            else
            {
                await AssertEntityHardDeleted(entity);
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_HardDeleteAtomicByExpression_EntitySoftDeleted()
    {
        var entityWithDifferentGuid = ValidEntity with { GuidField = Guid.NewGuid() };
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, entityWithDifferentGuid], default);
        insertResult.HasError.Should().BeFalse();
        var softDeleteResult = await Repository.SoftDelete(x => x.GuidField == ValidEntity.GuidField, default);
        softDeleteResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;

        var deleteResult = await Repository.HardDeleteAtomic(x => x.GuidField == ValidEntity.GuidField, default);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (entity.GuidField == ValidEntity.GuidField)
            {
                await AssertEntityHardDeleted(entity);
            }
            else
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, default);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_HardDeleteByIds_EntitiesSoftDeleted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var toDelete = entities.Select(x => x.Id).Take(2);
        var softDeleteResult = await Repository.SoftDelete(toDelete, default);
        softDeleteResult.HasError.Should().BeFalse();
        var toKeep = entities.Select(x => x.Id).Skip(2).Single();

        var deleteResult = await Repository.HardDelete(toDelete, default);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (toKeep == entity.Id)
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, default);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
            else
            {
                await AssertEntityHardDeleted(entity);
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_HardDeleteByExpression_EntitiesSoftDeleted()
    {
        var entityWithDifferentGuid = ValidEntity with { GuidField = Guid.NewGuid() };
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, entityWithDifferentGuid], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(x => x.GuidField == ValidEntity.GuidField, default);
        softDeleteResult.HasError.Should().BeFalse();

        var deleteResult = await Repository.HardDelete(x => x.GuidField == ValidEntity.GuidField, default);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (entity.GuidField == ValidEntity.GuidField)
            {
                await AssertEntityHardDeleted(entity);
            }
            else
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, default);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
        }
    }

    private async Task AssertEntityHardDeleted(TestEntity originalEntity)
    {
        var getEntityInRepoResult = await Repository.GetById(originalEntity.Id, default);
        getEntityInRepoResult.HasError.Should().BeTrue();
        getEntityInRepoResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();

        var actualEntity = await DbContext.TestEntities
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == originalEntity.Id);
        actualEntity.Should().BeNull();
    }
}
