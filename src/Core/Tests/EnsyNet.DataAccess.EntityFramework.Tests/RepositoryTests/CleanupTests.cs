using EnsyNet.Core.Results;
using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class CleanupTests : RepositoryTestsBase
{
    private readonly TestDataAccessCleanupService _cleanupService;
    
    public CleanupTests() : base()
    {
        _cleanupService = new TestDataAccessCleanupService(Repository, NullLogger<TestDataAccessCleanupService>.Instance);
    }

    [Fact]
    public async Task NoEntitiesSoftDeletedBeforeCutoff_NoEntitiesCleanedUp()
    {
        var result = await _cleanupService.CleanDataAccess(DateTime.MinValue, 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(0);
    }

    [Fact]
    public async Task SoftDeletedEntities_CleanupWithOverriddenMatchingFilter_OnlyCleanupEntitiesMatchingFilter()
    {
        var matchingGuid = Guid.NewGuid();
        var entitiesToInsert = new[]
        {
            ValidEntity with { GuidField = matchingGuid },
            ValidEntity with { GuidField = Guid.NewGuid() },
        };
        var insertResult = await Repository.Insert(entitiesToInsert, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entities.Select(x => x.Id), CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();

        _cleanupService.EntityFilter = x => x.GuidField == matchingGuid;
        var result = await _cleanupService.CleanDataAccess(DateTime.Now.AddMinutes(50), 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(1);
        await AssertEntityHardDeleted(entities.First());
    }

    [Fact]
    public async Task SoftDeletedEntity_CleanupWithOverriddenBeforeDelete_BeforeDeleteCalled()
    {
        var called = false;
        var insertResult = await Repository.Insert(ValidEntity, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entity.Id, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();

        _cleanupService.CanDeleteEntity = x =>
        {
            if (x.Id != entity.Id)
            {
                return Result.Ok(false);
            }
            called = true;
            return Result.Ok(true);
        };
        var result = await _cleanupService.CleanDataAccess(DateTime.Now.AddMinutes(50), 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(1);
        called.Should().BeTrue();
        await AssertEntityHardDeleted(entity);
    }

    [Fact]
    public async Task SoftDeletedEntity_CleanupWithOverriddenBeforeDeleteFailure_EntityNotDeleted()
    {
        var insertResult = await Repository.Insert(ValidEntity, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entity.Id, CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();

        _cleanupService.CanDeleteEntity = x =>
        {
            return Result.Ok(false);
        };
        var result = await _cleanupService.CleanDataAccess(DateTime.Now.AddMinutes(50), 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(0);
    }

    [Fact]
    public async Task ManySoftDeletedEntities_EntitiesCleanedUp()
    {
        var entitiesToInsert = Enumerable.Range(0, 500).Select(x => ValidEntity with { StringField = $"Entity {x}" });
        var insertResult = await Repository.Insert(entitiesToInsert, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entities.Select(x => x.Id), CancellationToken.None);
        softDeleteResult.HasError.Should().BeFalse();

        var result = await _cleanupService.CleanDataAccess(DateTime.Now.AddMinutes(50), 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(500);
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
