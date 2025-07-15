using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Models;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests.BaseRepositoryTests;

public class GetTests : RepositoryTestsBase
{
    [Fact]
    public async Task EntityInserted_GetById_EntityRetrieved()
    {
        var insertResult = await Repository.Insert(ValidEntity, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;

        var getResult = await Repository.GetById(entity.Id, CancellationToken.None);

        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, ValidEntity);
    }

    [Fact]
    public async Task NoEntityInDb_GetById_EntityNotFound()
    {
        var getResult = await Repository.GetById(Guid.NewGuid(), CancellationToken.None);

        getResult.HasError.Should().BeTrue();
        getResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();
    }

    [Fact]
    public async Task EntityInserted_GetByExpression_EntityRetrieved()
    {
        var entityToInsert = ValidEntity with { StringField = Guid.NewGuid().ToString() };
        var insertResult = await Repository.Insert(entityToInsert, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getResult = await Repository.GetByExpression(x => x.StringField == entityToInsert.StringField, CancellationToken.None);
        
        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, entityToInsert);
    }

    [Fact]
    public async Task NoEntityInDb_GetByExpression_EntityNotFound()
    {
        var getResult = await Repository.GetByExpression(x => x.StringField == "nonexistentStringField", CancellationToken.None);
        
        getResult.HasError.Should().BeTrue();
        getResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();
    }

    [Fact]
    public async Task EntitiesInserted_GetAll_EntitiesRetrieved()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetAll(CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        var retrievedEntities = getAllResult.Data!.Where(x => insertResult.Data!.Select(r => r.Id).Contains(x.Id));
        foreach (var entity in retrievedEntities)
        {
            AssertEntity(entity, ValidEntity);
        }
    }

    [Fact]
    public async Task EntitiesInserted_GetAllSortedAscending_EntitiesRetrieved()
    {
        var entity1 = ValidEntity with { StringField = "A" };
        var entity2 = ValidEntity with { StringField = "B" };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetAll(new() { SortFieldSelector = x => x.StringField, IsAscending = true }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        AssertEntity(getAllResult.Data!.Where(x => x.GuidField == ValidEntity.GuidField).ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.Where(x => x.GuidField == ValidEntity.GuidField).ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetAllSortedDescending_EntitiesRetrieved()
    {
        var entity1 = ValidEntity with { StringField = "Z" };
        var entity2 = ValidEntity with { StringField = "X" };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetAll(new() { SortFieldSelector = x => x.StringField, IsAscending = false }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyPaged_EntitiesRetrieved()
    {
        var insertResult = await Repository.Insert([ValidEntity, ValidEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetMany(new() { Skip = 0, Take = 1 }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
    }

    [Fact]
    public async Task EntitiesInserted_GetManySortedAndPaged_EntitiesRetrieved()
    {
        var entity1 = ValidEntity with { StringField = "_A" };
        var entity2 = ValidEntity with { StringField = "_B" };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult1 = await Repository.GetMany(
            new() { Skip = 0, Take = 1 },
            new() { SortFieldSelector = x => x.StringField, IsAscending = true },
            CancellationToken.None);
        var getAllResult2 = await Repository.GetMany(
            new() { Skip = 1, Take = 1 },
            new() { SortFieldSelector = x => x.StringField, IsAscending = true },
            CancellationToken.None);

        getAllResult1.HasError.Should().BeFalse();
        getAllResult1.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult1.Data!.Single(), entity1);
        getAllResult2.HasError.Should().BeFalse();
        getAllResult2.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult2.Data!.Single(), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpression_EntitiesRetrieved()
    {
        var commonGuid = Guid.NewGuid();
        var entity1 = ValidEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };
        var entity2 = ValidEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetManyByExpression(x => x.GuidField == commonGuid, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.Single(x => x.Id == insertResult.Data!.ElementAt(0).Id), entity1);
        AssertEntity(getAllResult.Data!.Single(x => x.Id == insertResult.Data!.ElementAt(1).Id), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionPaginated_EntitiesRetrieved()
    {
        var commonGuid = Guid.NewGuid();
        var entity1 = ValidEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };
        var entity2 = ValidEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetManyByExpression(x => x.GuidField == commonGuid, new PaginationQuery { Skip = 0, Take = 1 }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedAscending_EntitiesRetrieved()
    {
        var commonGuid = Guid.NewGuid();
        var entity1 = ValidEntity with { GuidField = commonGuid, StringField = "A" };
        var entity2 = ValidEntity with { GuidField = commonGuid, StringField = "B" };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetManyByExpression(
            x => x.GuidField == commonGuid,
            new SortingQuery<TestEntity> { SortFieldSelector = y => y.StringField, IsAscending = true },
            CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedDescending_EntitiesRetrieved()
    {
        var commonGuid = Guid.NewGuid();
        var entity1 = ValidEntity with { GuidField = commonGuid, StringField = "A" };
        var entity2 = ValidEntity with { GuidField = commonGuid, StringField = "B" };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetManyByExpression(
            x => x.GuidField == commonGuid,
            new SortingQuery<TestEntity> { SortFieldSelector = y => y.StringField, IsAscending = false },
            CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedAndPaginated_EntitiesRetrieved()
    {
        var commonGuid = Guid.NewGuid();
        var entity1 = ValidEntity with { GuidField = commonGuid, StringField = "A" };
        var entity2 = ValidEntity with { GuidField = commonGuid, StringField = "B" };
        var insertResult = await Repository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await Repository.GetManyByExpression(
            x => x.GuidField == commonGuid,
            new() { Skip = 1, Take = 1 },
            new() { SortFieldSelector = y => y.StringField, IsAscending = true },
            CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity2);
    }
}
