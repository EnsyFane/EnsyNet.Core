using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Models;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests.PartitionedRepositoryTests;

public class GetTests : RepositoryTestsBase
{
    [Fact]
    public async Task EntityInserted_GetById_EntityRetrieved()
    {
        var insertResult = await PartitionedRepository.Insert(ValidPartitionedEntity, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;

        var getResult = await PartitionedRepository.GetById(ValidPartitionedEntity.OrgId, entity.Id, CancellationToken.None);

        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, ValidPartitionedEntity);
    }

    [Fact]
    public async Task NoEntityInDb_GetById_EntityNotFound()
    {
        var getResult = await PartitionedRepository.GetById(Guid.NewGuid(),  Guid.NewGuid(), CancellationToken.None);

        getResult.HasError.Should().BeTrue();
        getResult.Error.Should().BeOfType<EntityNotFoundError<PartitionedTestEntity>>();
    }

    [Fact]
    public async Task EntityInserted_GetByExpression_EntityRetrieved()
    {
        var entityToInsert = ValidPartitionedEntity with { Name = Guid.NewGuid().ToString() };
        var insertResult = await PartitionedRepository.Insert(entityToInsert, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getResult = await PartitionedRepository.GetByExpression(ValidPartitionedEntity.OrgId, x => x.Name == entityToInsert.Name, CancellationToken.None);
        
        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, entityToInsert);
    }

    [Fact]
    public async Task NoEntityInDb_GetByExpression_EntityNotFound()
    {
        var getResult = await PartitionedRepository.GetByExpression( Guid.NewGuid(), x => x.Name == "nonexistentStringField", CancellationToken.None);
        
        getResult.HasError.Should().BeTrue();
        getResult.Error.Should().BeOfType<EntityNotFoundError<PartitionedTestEntity>>();
    }

    [Fact]
    public async Task EntitiesInserted_GetAll_EntitiesRetrieved()
    {
        var insertResult = await PartitionedRepository.Insert([ValidPartitionedEntity, ValidPartitionedEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetAll(ValidPartitionedEntity.OrgId, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        var retrievedEntities = getAllResult.Data!.Where(x => insertResult.Data!.Select(r => r.Id).Contains(x.Id));
        foreach (var entity in retrievedEntities)
        {
            AssertEntity(entity, ValidPartitionedEntity);
        }
    }

    [Fact]
    public async Task EntitiesInserted_GetAllSortedAscending_EntitiesRetrieved()
    {
        var entity1 = ValidPartitionedEntity with { Name = "A" };
        var entity2 = ValidPartitionedEntity with { Name = "B" };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetAll(ValidPartitionedEntity.OrgId, new() { SortFieldSelector = x => x.Name, IsAscending = true }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetAllSortedDescending_EntitiesRetrieved()
    {
        var entity1 = ValidPartitionedEntity with { Name = "Z" };
        var entity2 = ValidPartitionedEntity with { Name = "X" };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetAll(ValidPartitionedEntity.OrgId, new() { SortFieldSelector = x => x.Name, IsAscending = false }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyPaged_EntitiesRetrieved()
    {
        var insertResult = await PartitionedRepository.Insert([ValidPartitionedEntity, ValidPartitionedEntity], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetMany(ValidPartitionedEntity.OrgId, new() { Skip = 0, Take = 1 }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
    }

    [Fact]
    public async Task EntitiesInserted_GetManySortedAndPaged_EntitiesRetrieved()
    {
        var entity1 = ValidPartitionedEntity with { Name = "_A" };
        var entity2 = ValidPartitionedEntity with { Name = "_B" };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult1 = await PartitionedRepository.GetMany(
            ValidPartitionedEntity.OrgId,
            new() { Skip = 0, Take = 1 },
            new() { SortFieldSelector = x => x.Name, IsAscending = true },
            CancellationToken.None);
        var getAllResult2 = await PartitionedRepository.GetMany(
            ValidPartitionedEntity.OrgId,
            new() { Skip = 1, Take = 1 },
            new() { SortFieldSelector = x => x.Name, IsAscending = true },
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
        var commonValue = Random.Shared.Next();
        var entity1 = ValidPartitionedEntity with { Value = commonValue, Name = Guid.NewGuid().ToString() };
        var entity2 = ValidPartitionedEntity with { Value = commonValue, Name = Guid.NewGuid().ToString() };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetManyByExpression(entity1.OrgId, x => x.Value == commonValue, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.Single(x => x.Id == insertResult.Data!.ElementAt(0).Id), entity1);
        AssertEntity(getAllResult.Data!.Single(x => x.Id == insertResult.Data!.ElementAt(1).Id), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionPaginated_EntitiesRetrieved()
    {
        var commonValue = Random.Shared.Next();
        var entity1 = ValidPartitionedEntity with { Value = commonValue, Name = Guid.NewGuid().ToString() };
        var entity2 = ValidPartitionedEntity with { Value = commonValue, Name = Guid.NewGuid().ToString() };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetManyByExpression(ValidPartitionedEntity.OrgId, x => x.Value == commonValue, new PaginationQuery { Skip = 0, Take = 1 }, CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedAscending_EntitiesRetrieved()
    {
        var commonValue = Random.Shared.Next();
        var entity1 = ValidPartitionedEntity with { Value = commonValue, Name = "A" };
        var entity2 = ValidPartitionedEntity with { Value = commonValue, Name = "B" };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetManyByExpression(
            ValidPartitionedEntity.OrgId,
            x => x.Value == commonValue,
            new SortingQuery<PartitionedTestEntity> { SortFieldSelector = y => y.Name, IsAscending = true },
            CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedDescending_EntitiesRetrieved()
    {
        var commonValue = Random.Shared.Next();
        var entity1 = ValidPartitionedEntity with { Value = commonValue, Name = "A" };
        var entity2 = ValidPartitionedEntity with { Value = commonValue, Name = "B" };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetManyByExpression(
            ValidPartitionedEntity.OrgId,
            x => x.Value == commonValue,
            new SortingQuery<PartitionedTestEntity> { SortFieldSelector = y => y.Name, IsAscending = false },
            CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedAndPaginated_EntitiesRetrieved()
    {
        var commonValue = Random.Shared.Next();
        var entity1 = ValidPartitionedEntity with { Value = commonValue, Name = "A" };
        var entity2 = ValidPartitionedEntity with { Value = commonValue, Name = "B" };
        var insertResult = await PartitionedRepository.Insert([entity1, entity2], CancellationToken.None);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await PartitionedRepository.GetManyByExpression(
            ValidPartitionedEntity.OrgId,
            x => x.Value == commonValue,
            new() { Skip = 1, Take = 1 },
            new() { SortFieldSelector = y => y.Name, IsAscending = true },
            CancellationToken.None);
        
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity2);
    }


    [Fact]
    public async Task EntitiesDifferentOrgs_GetById_OneEntityRetrieved()
    {
        var insertResult = await PartitionedRepository.Insert(ValidPartitionedEntity, CancellationToken.None);
        var insertResultDifferentOrg = await PartitionedRepository.Insert(ValidPartitionedEntity with { OrgId = Guid.NewGuid() }, CancellationToken.None);
        insertResult.HasError.Should().BeFalse();
        insertResultDifferentOrg.HasError.Should().BeFalse();
        var entity = insertResult.Data!;
        var entityDifferentOrg = insertResultDifferentOrg.Data!;

        var getResult = await PartitionedRepository.GetById(ValidPartitionedEntity.OrgId, entity.Id, CancellationToken.None);
        var getResultDifferentOrg = await PartitionedRepository.GetById(ValidPartitionedEntity.OrgId, entityDifferentOrg.Id, CancellationToken.None);

        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, ValidPartitionedEntity);
        getResultDifferentOrg.HasError.Should().BeTrue();
        getResultDifferentOrg.Error.Should().BeOfType<EntityNotFoundError<PartitionedTestEntity>>();
    }
}
