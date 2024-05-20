using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;
using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class SoftDeleteTests : RepositoryTestsBase
{
    [Fact]
    public async Task EntityInserted_SoftDeleteById_EntitySoftDeleted()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;

        var deleteResult = await Repository.SoftDelete(entity.Id, default);
        
        deleteResult.HasError.Should().BeFalse();
        await AssertEntitySoftDeleted(entity);
    }

    [Fact]
    public async Task SoftDeletedEntity_SoftDelete_SecondSoftDeleteFails()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        var firstDeleteResult = await Repository.SoftDelete(entity.Id, default);
        firstDeleteResult.HasError.Should().BeFalse();
        await AssertEntitySoftDeleted(entity);

        var secondDeleteResult = await Repository.SoftDelete(entity.Id, default);
        
        secondDeleteResult.HasError.Should().BeTrue();
        secondDeleteResult.Error.Should().BeOfType<DeleteOperationFailedError>();
    }

    [Fact]
    public async Task EntitiesInserted_SoftDeleteAtomicByIds_EntitySoftDeleted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var toDelete = entities.Select(x => x.Id).Take(2);
        var toKeep = entities.Select(x => x.Id).Skip(2).Single();

        var deleteResult = await Repository.SoftDeleteAtomic(toDelete, default);
        
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
                await AssertEntitySoftDeleted(entity);
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_SoftDeleteAtomicByExpression_EntitySoftDeleted()
    {
        var entityWithDifferentGuid = ValidEntity with { GuidField = Guid.NewGuid() };
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, entityWithDifferentGuid], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;

        var deleteResult = await Repository.SoftDeleteAtomic(x => x.GuidField == ValidEntity.GuidField, default);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (entity.GuidField == ValidEntity.GuidField)
            {
                await AssertEntitySoftDeleted(entity);
            }
            else
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, default);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_SoftDeleteByIds_EntitiesSoftDeleted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var toDelete = entities.Select(x => x.Id).Take(2);
        var toKeep = entities.Select(x => x.Id).Skip(2).Single();

        var deleteResult = await Repository.SoftDelete(toDelete, default);
        
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
                await AssertEntitySoftDeleted(entity);
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_SoftDeleteByExpression_EntitiesSoftDeleted()
    {
        var entityWithDifferentGuid = ValidEntity with { GuidField = Guid.NewGuid() };
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, entityWithDifferentGuid], default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;

        var deleteResult = await Repository.SoftDelete(x => x.GuidField == ValidEntity.GuidField, default);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (entity.GuidField == ValidEntity.GuidField)
            {
                await AssertEntitySoftDeleted(entity);
            }
            else
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, default);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
        }
    }

    private async Task AssertEntitySoftDeleted(TestEntity originalEntity)
    {
        var getEntityInRepoResult = await Repository.GetById(originalEntity.Id, default);
        getEntityInRepoResult.HasError.Should().BeTrue();
        getEntityInRepoResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();

        var actualEntity = await DbContext.TestEntities
            .IgnoreQueryFilters()
            .SingleAsync(x => x.Id == originalEntity.Id);
        actualEntity.DeletedAt.Should().NotBeNull();
        actualEntity.DeletedAt.Should().BeCloseTo(originalEntity.CreatedAt, TimeSpan.FromSeconds(1));
    }
}
