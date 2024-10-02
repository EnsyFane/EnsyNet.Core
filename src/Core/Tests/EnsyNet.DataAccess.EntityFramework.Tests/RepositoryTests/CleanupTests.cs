using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;

using FluentAssertions;

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
        var insertResult = await Repository.Insert(entitiesToInsert, default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entities.Select(x => x.Id), default);
        softDeleteResult.HasError.Should().BeFalse();

        _cleanupService.EntityFilter = x => x.GuidField == matchingGuid;
        var result = await _cleanupService.CleanDataAccess(DateTime.Now.AddMinutes(50), 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(1);
    }

    [Fact]
    public async Task ManySoftDeletedEntities_EntitiesCleanedUp()
    {
        var entitiesToInsert = Enumerable.Range(0, 500).Select(x => ValidEntity with { StringField = $"Entity {x}" });
        var insertResult = await Repository.Insert(entitiesToInsert, default);
        insertResult.HasError.Should().BeFalse();
        var entities = insertResult.Data!;
        var softDeleteResult = await Repository.SoftDelete(entities.Select(x => x.Id), default);
        softDeleteResult.HasError.Should().BeFalse();

        var result = await _cleanupService.CleanDataAccess(DateTime.Now.AddMinutes(50), 500, CancellationToken.None);

        result.HasError.Should().BeFalse();
        result.Data.Should().Be(500);
    }
}
