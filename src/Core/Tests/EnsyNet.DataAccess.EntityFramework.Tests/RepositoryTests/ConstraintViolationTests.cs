using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class ConstraintViolationTests : RepositoryTestsBase
{
    [Fact]
    public async Task ExistingEntity_InsertWithSameUniqueField_ReturnsUniqueConstraintViolationError()
    {
        var uniqueValue = Guid.NewGuid().ToString();
        var insertResult = await Repository.Insert(ValidEntity with { UniqueField = uniqueValue }, CancellationToken.None);
        insertResult.AssertNoError();

        var duplicateInsertResult = await Repository.Insert(ValidEntity with { GuidField = Guid.NewGuid(), UniqueField = uniqueValue }, CancellationToken.None);

        duplicateInsertResult.HasError.Should().BeTrue();
        duplicateInsertResult.Error.Should().BeOfType<UniqueConstraintViolationError>();
    }

    [Fact]
    public async Task ExistingEntitiesWithDependent_HardDeleteParent_ReturnsForeignKeyConstraintViolationError()
    {
        var insertResult = await Repository.Insert(ValidEntity, CancellationToken.None);
        insertResult.AssertNoError();
        var parent = insertResult.Data!;
        var insertChildResult = await ChildRepository.Insert(new ChildTestEntity { ParentId = parent.Id }, CancellationToken.None);
        insertChildResult.AssertNoError();

        var deleteResult = await Repository.HardDelete(parent.Id, CancellationToken.None);

        deleteResult.HasError.Should().BeTrue();
        deleteResult.Error.Should().BeOfType<ForeignKeyConstraintViolationError>();
    }
}
