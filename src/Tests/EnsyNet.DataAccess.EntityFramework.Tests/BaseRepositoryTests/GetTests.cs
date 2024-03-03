using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.Abstractions.Models;
using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.BaseRepositoryTests;

public class GetTests
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
    public async Task EntityInserted_GetById_EntityRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var insertResult = await repository.Insert(_validEntity, default);
        insertResult.HasError.Should().BeFalse();
        var entity = insertResult.Data!;

        var getResult = await repository.GetById(entity.Id, default);
        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, _validEntity);
    }

    [Fact]
    public async Task NoEntityInDb_GetById_EntityNotFound()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var getResult = await repository.GetById(Guid.NewGuid(), default);
        getResult.HasError.Should().BeTrue();
        getResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();
    }

    [Fact]
    public async Task EntityInserted_GetByExpression_EntityRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entityToInsert = _validEntity with { StringField = Guid.NewGuid().ToString() };

        var insertResult = await repository.Insert(entityToInsert, default);
        insertResult.HasError.Should().BeFalse();

        var getResult = await repository.GetByExpression(x => x.StringField == entityToInsert.StringField, default);
        getResult.HasError.Should().BeFalse();
        var retrievedEntity = getResult.Data!;
        AssertEntity(retrievedEntity, entityToInsert);
    }

    [Fact]
    public async Task NoEntityInDb_GetByExpression_EntityNotFound()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var getResult = await repository.GetByExpression(x => x.StringField == "inexistentStringField", default);
        getResult.HasError.Should().BeTrue();
        getResult.Error.Should().BeOfType<EntityNotFoundError<TestEntity>>();
    }

    [Fact]
    public async Task EntitiesInserted_GetAll_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var insertResult = await repository.Insert([_validEntity, _validEntity], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetAll(default);
        getAllResult.HasError.Should().BeFalse();
        var retrievedEntities = getAllResult.Data!.Where(x => insertResult.Data!.Select(r => r.Id).Contains(x.Id));
        foreach(var entity in retrievedEntities)
        {
            AssertEntity(entity, _validEntity);
        }
    }

    [Fact]
    public async Task EntitiesInserted_GetAllSortedAscending_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entity1 = _validEntity with { StringField = "A" };
        var entity2 = _validEntity with { StringField = "B" };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetAll(new SortingQuery<TestEntity> { SortFieldSelector = x => x.StringField, IsAscending = true }, default);
        getAllResult.HasError.Should().BeFalse();
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetAllSortedDescending_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entity1 = _validEntity with { StringField = "Z" };
        var entity2 = _validEntity with { StringField = "X" };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetAll(new SortingQuery<TestEntity> { SortFieldSelector = x => x.StringField, IsAscending = false }, default);
        getAllResult.HasError.Should().BeFalse();
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyPaged_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);

        var insertResult = await repository.Insert([_validEntity, _validEntity], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetMany(new PaginationQuery { Skip = 0, Take = 1 }, default);
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
    }

    [Fact]
    public async Task EntitiesInserted_GetManySortedAndPaged_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var entity1 = _validEntity with { StringField = "_A" };
        var entity2 = _validEntity with { StringField = "_B" };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult1 = await repository.GetMany(
            new PaginationQuery { Skip = 0, Take = 1 },
            new SortingQuery<TestEntity> { SortFieldSelector = x => x.StringField, IsAscending = true },
            default);
        getAllResult1.HasError.Should().BeFalse();
        getAllResult1.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult1.Data!.Single(), entity1);

        var getAllResult2 = await repository.GetMany(
            new PaginationQuery { Skip = 1, Take = 1 },
            new SortingQuery<TestEntity> { SortFieldSelector = x => x.StringField, IsAscending = true },
            default);
        getAllResult2.HasError.Should().BeFalse();
        getAllResult2.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult2.Data!.Single(), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpression_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var commonGuid = Guid.NewGuid();
        var entity1 = _validEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };
        var entity2 = _validEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetManyByExpression(x => x.GuidField == commonGuid, default);
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.Single(x => x.Id == insertResult.Data!.ElementAt(0).Id), entity1);
        AssertEntity(getAllResult.Data!.Single(x => x.Id == insertResult.Data!.ElementAt(1).Id), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionPaginated_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var commonGuid = Guid.NewGuid();
        var entity1 = _validEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };
        var entity2 = _validEntity with { GuidField = commonGuid, StringField = Guid.NewGuid().ToString() };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetManyByExpression(x => x.GuidField == commonGuid, new PaginationQuery { Skip = 0, Take = 1 }, default);
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedAscending_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var commonGuid = Guid.NewGuid();
        var entity1 = _validEntity with { GuidField = commonGuid, StringField = "A" };
        var entity2 = _validEntity with { GuidField = commonGuid, StringField = "B" };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetManyByExpression(
            x => x.GuidField == commonGuid,
            new SortingQuery<TestEntity> { SortFieldSelector = y => y.StringField, IsAscending = true },
            default);
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedDescending_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var commonGuid = Guid.NewGuid();
        var entity1 = _validEntity with { GuidField = commonGuid, StringField = "A" };
        var entity2 = _validEntity with { GuidField = commonGuid, StringField = "B" };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetManyByExpression(
            x => x.GuidField == commonGuid,
            new SortingQuery<TestEntity> { SortFieldSelector = y => y.StringField, IsAscending = false },
            default);
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(2);
        AssertEntity(getAllResult.Data!.ElementAt(1), entity1);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity2);
    }

    [Fact]
    public async Task EntitiesInserted_GetManyByExpressionSortedAndPaginated_EntitiesRetrieved()
    {
        using var context = new TestDbContext(DatabaseConfiguration.Options);
        var repository = new TestRepository(context);
        var commonGuid = Guid.NewGuid();
        var entity1 = _validEntity with { GuidField = commonGuid, StringField = "A" };
        var entity2 = _validEntity with { GuidField = commonGuid, StringField = "B" };

        var insertResult = await repository.Insert([entity1, entity2], default);
        insertResult.HasError.Should().BeFalse();

        var getAllResult = await repository.GetManyByExpression(
            x => x.GuidField == commonGuid,
            new PaginationQuery { Skip = 1, Take = 1 },
            new SortingQuery<TestEntity> { SortFieldSelector = y => y.StringField, IsAscending = true },
            default);
        getAllResult.HasError.Should().BeFalse();
        getAllResult.Data!.Count().Should().Be(1);
        AssertEntity(getAllResult.Data!.ElementAt(0), entity2);
    }

    private static void AssertEntity(TestEntity actualEntity, TestEntity expectedEntity)
    {
        actualEntity.Id.Should().NotBe(Guid.Empty);
        actualEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        actualEntity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
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
