using EnsyNet.DataAccess.Abstractions.Errors;
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
        var insertResult = await Repository.Insert(ValidEntity, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;

        var deleteResult = await Repository.HardDelete(entity.Id, CancellationToken.None);
        
        deleteResult.HasError.Should().BeFalse();
        await AssertEntityHardDeleted(entity);
    }

    [Fact]
    public async Task SoftDeletedEntity_HardDelete_EntityHardDeleted()
    {
        var insertResult = await Repository.Insert(ValidEntity, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entity.Id, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();

        var hardDeleteResult = await Repository.HardDelete(entity.Id, CancellationToken.None);
        
        hardDeleteResult.HasError.Should().BeFalse();
        await AssertEntityHardDeleted(entity);
    }

    [Fact]
    public async Task EntitiesInserted_HardDeleteAtomicByIds_EntityHardDeleted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!.ToList();
        var toDelete = entities.Select(x => x.Id).Take(2).ToList();
        var softDeleteResult = await Repository.SoftDelete(toDelete, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();
        var toKeep = entities.Select(x => x.Id).Skip(2).Single();

        var deleteResult = await Repository.HardDeleteAtomic(toDelete, CancellationToken.None);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (toKeep == entity.Id)
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, CancellationToken.None);
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
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, entityWithDifferentGuid], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var softDeleteResult = await Repository.SoftDelete(x => x.GuidField == ValidEntity.GuidField, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;

        var deleteResult = await Repository.HardDeleteAtomic(x => x.GuidField == ValidEntity.GuidField, CancellationToken.None);
        
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
                var getEntityInRepoResult = await Repository.GetById(entity.Id, CancellationToken.None);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
        }
    }

    [Fact]
    public async Task EntitiesInserted_HardDeleteByIds_EntitiesSoftDeleted()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!.ToList();
        var toDelete = entities.Select(x => x.Id).Take(2).ToList();
        var softDeleteResult = await Repository.SoftDelete(toDelete, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();
        var toKeep = entities.Select(x => x.Id).Skip(2).Single();

        var deleteResult = await Repository.HardDelete(toDelete, CancellationToken.None);
        
        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
        foreach (var entity in entities)
        {
            if (toKeep == entity.Id)
            {
                var getEntityInRepoResult = await Repository.GetById(entity.Id, CancellationToken.None);
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
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, entityWithDifferentGuid], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(x => x.GuidField == ValidEntity.GuidField, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();

        var deleteResult = await Repository.HardDelete(x => x.GuidField == ValidEntity.GuidField, CancellationToken.None);
        
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
                var getEntityInRepoResult = await Repository.GetById(entity.Id, CancellationToken.None);
                getEntityInRepoResult.HasError.Should().BeFalse();
            }
        }
    }

    [Fact]
    public async Task NoEntity_HardDeleteById_ReturnsError()
    {
        var deleteResult = await Repository.HardDelete(Guid.NewGuid(), CancellationToken.None);

        deleteResult.HasError.Should().BeTrue();
        deleteResult.Error.Should().BeOfType<DeleteOperationFailedError>();
    }

    [Fact]
    public async Task NoEntities_HardDeleteByIds_ReturnsError()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var deleteResult = await Repository.HardDelete(ids, CancellationToken.None);

        deleteResult.HasError.Should().BeTrue();
        deleteResult.Error.Should().BeOfType<BulkDeleteOperationFailedError>();
    }

    [Fact]
    public async Task PartialEntities_HardDeleteByIds_SucceedsWithPartialCount()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity, ValidEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!.ToList();
        var firstDeleteResult = await Repository.HardDelete(entities[0].Id, CancellationToken.None);
        firstDeleteResult.HasError.Should().BeFalse();

        var deleteResult = await Repository.HardDelete(entities.Select(x => x.Id), CancellationToken.None);

        deleteResult.HasError.Should().BeFalse();
        deleteResult.Data.Should().Be(2);
    }

    [Fact]
    public async Task NoEntities_HardDeleteByExpression_ReturnsError()
    {
        var deleteResult = await Repository.HardDelete(x => x.StringField == "nonexistent-value", CancellationToken.None);

        deleteResult.HasError.Should().BeTrue();
        deleteResult.Error.Should().BeOfType<BulkDeleteOperationFailedError>();
    }

    [Fact]
    public async Task NoEntities_HardDeleteAtomicByIds_ReturnsError()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var deleteResult = await Repository.HardDeleteAtomic(ids, CancellationToken.None);

        deleteResult.HasError.Should().BeTrue();
        deleteResult.Error.Should().BeOfType<BulkDeleteOperationFailedError>();
    }

    [Fact]
    public async Task NoEntities_HardDeleteAtomicByExpression_ReturnsError()
    {
        var deleteResult = await Repository.HardDeleteAtomic(x => x.StringField == "nonexistent-value", CancellationToken.None);

        deleteResult.HasError.Should().BeTrue();
        deleteResult.Error.Should().BeOfType<BulkDeleteOperationFailedError>();
    }

    private async Task AssertEntityHardDeleted(TestEntity originalEntity)
    {
        var getEntityInRepoResult = await Repository.GetById(originalEntity.Id, CancellationToken.None);
        getEntityInRepoResult.HasError.Should().BeTrue();
        getEntityInRepoResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();

        var actualEntity = await DbContext.TestEntities
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == originalEntity.Id);
        actualEntity.Should().BeNull();
    }
}
