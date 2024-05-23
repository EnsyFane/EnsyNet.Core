﻿using EnsyNet.DataAccess.Abstractions.Errors;
using EnsyNet.DataAccess.EntityFramework.Tests.Helpers;
using EnsyNet.DataAccess.EntityFramework.Tests.Models;

using FluentAssertions;

using Microsoft.EntityFrameworkCore.Query;

using System.Linq.Expressions;

using Xunit;

namespace EnsyNet.DataAccess.EntityFramework.Tests.RepositoryTests;

public class UpdateTests : RepositoryTestsBase
{
    [Fact]
    public async Task ExistingEntity_UpdateById_EntityUpdated()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();
        var entity = insertResult.Data!;
        var dateTime = DateTime.UtcNow.AddDays(5);
        var guid = Guid.NewGuid();
        await Task.Delay(TimeSpan.FromSeconds(1));

        var updateResult = await Repository.Update(
            entity.Id,
            x => x.SetProperty(e => e.StringField, "Updated")
                .SetProperty(e => e.IntField, 2602)
                .SetProperty(e => e.BoolField, false)
                .SetProperty(e => e.FloatField, 42.42f)
                .SetProperty(e => e.DoubleField, 42.42d)
                .SetProperty(e => e.DecimalField, 42.42m)
                .SetProperty(e => e.DateTimeField, dateTime)
                .SetProperty(e => e.TimeSpanField, TimeSpan.FromSeconds(5))
                .SetProperty(e => e.GuidField, guid),
            default);

        updateResult.AssertNoError();
        var updatedEntity = await Repository.GetById(entity.Id, default);
        AssertEntity(updatedEntity.Data!, new TestEntity
        {
            StringField = "Updated",
            IntField = 2602,
            BoolField = false,
            FloatField = 42.42f,
            DoubleField = 42.42d,
            DecimalField = 42.42m,
            DateTimeField = dateTime,
            TimeSpanField = TimeSpan.FromSeconds(5),
            GuidField = guid,
        }, assertAutoGenerated: false);
        updatedEntity.Data!.UpdatedAt.Should().NotBeCloseTo(entity.UpdatedAt!.Value, TimeSpan.FromSeconds(0.8d));
        updatedEntity.Data!.UpdatedAt.Should().BeAfter(entity.UpdatedAt!.Value);
    }

    [Fact]
    public async Task NoEntity_UpdateById_ReturnsError()
    {
        var updateResult = await Repository.Update(
            Guid.NewGuid(),
            x => x.SetProperty(e => e.StringField, "Updated"),
            default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<UpdateOperationFailedError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateByIdCreatedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();
        var entity = insertResult.Data!;

        var updateResult = await Repository.Update(
            entity.Id,
            x => x.SetProperty(e => e.CreatedAt, DateTime.UtcNow.AddDays(-5)),
            default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateByIdUpdatedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();
        var entity = insertResult.Data!;

        var updateResult = await Repository.Update(
            entity.Id,
            x => x.SetProperty(e => e.UpdatedAt, DateTime.UtcNow.AddDays(-5)),
            default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateByIdDeletedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();
        var entity = insertResult.Data!;

        var updateResult = await Repository.Update(
            entity.Id,
            x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow.AddDays(-5)),
            default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntities_UpdateMultiple_EntitiesUpdated()
    {
        var insertResult1 = await Repository.Insert(ValidEntity, default);
        insertResult1.AssertNoError();
        var insertResult2 = await Repository.Insert(ValidEntity, default);
        insertResult2.AssertNoError();
        await Task.Delay(TimeSpan.FromSeconds(1));

        var updateResult = await Repository.Update(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult1.Data!.Id, x => x.SetProperty(e => e.StringField, "Updated1") },
                { insertResult2.Data!.Id, x => x.SetProperty(e => e.StringField, "Updated2") }
            }, default);

        updateResult.AssertNoError();
        var updatedEntity1 = await Repository.GetById(insertResult1.Data!.Id, default);
        updatedEntity1.Data!.StringField.Should().Be("Updated1");
        updatedEntity1.Data!.UpdatedAt.Should().NotBeCloseTo(insertResult1.Data.UpdatedAt!.Value, TimeSpan.FromSeconds(0.8d));
        updatedEntity1.Data!.UpdatedAt.Should().BeAfter(insertResult1.Data.UpdatedAt!.Value);
        var updatedEntity2 = await Repository.GetById(insertResult2.Data!.Id, default);
        updatedEntity2.Data!.StringField.Should().Be("Updated2");
        updatedEntity2.Data!.UpdatedAt.Should().NotBeCloseTo(insertResult2.Data.UpdatedAt!.Value, TimeSpan.FromSeconds(0.8d));
        updatedEntity2.Data!.UpdatedAt.Should().BeAfter(insertResult2.Data.UpdatedAt!.Value);
    }

    [Fact]
    public async Task NoEntities_UpdateMultiple_ReturnsError()
    {
        var updateResult = await Repository.Update(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { Guid.NewGuid(), x => x.SetProperty(e => e.StringField, "Updated") },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<BulkUpdateOperationFailedError>();
    }

    [Fact]
    public async Task OneEntity_UpdateMultiple_ExistingEntityUpdated()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.Update(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.StringField, "Updated1") },
                { Guid.NewGuid(), x => x.SetProperty(e => e.StringField, "Updated2") },
            }, default);

        updateResult.AssertNoError();
        var updatedEntity = await Repository.GetById(insertResult.Data!.Id, default);
        updatedEntity.Data!.StringField.Should().Be("Updated1");
    }

    [Fact]
    public async Task ExistingEntity_UpdateMultipleCreatedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.Update(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.CreatedAt, DateTime.UtcNow.AddDays(-5)) },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateMultipleUpdatedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.Update(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.UpdatedAt, DateTime.UtcNow.AddDays(-5)) },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateMultipleDeletedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.Update(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow.AddDays(-5)) },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntities_UpdateAtomicMultiple_EntitiesUpdated()
    {
        var insertResult1 = await Repository.Insert(ValidEntity, default);
        insertResult1.AssertNoError();
        var insertResult2 = await Repository.Insert(ValidEntity, default);
        insertResult2.AssertNoError();
        await Task.Delay(TimeSpan.FromSeconds(1));

        var updateResult = await Repository.UpdateAtomic(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult1.Data!.Id, x => x.SetProperty(e => e.StringField, "Updated1") },
                { insertResult2.Data!.Id, x => x.SetProperty(e => e.StringField, "Updated2") }
            }, default);

        updateResult.AssertNoError();
        var updatedEntity1 = await Repository.GetById(insertResult1.Data!.Id, default);
        updatedEntity1.Data!.StringField.Should().Be("Updated1");
        updatedEntity1.Data!.UpdatedAt.Should().NotBeCloseTo(insertResult1.Data.UpdatedAt!.Value, TimeSpan.FromSeconds(0.8d));
        updatedEntity1.Data!.UpdatedAt.Should().BeAfter(insertResult1.Data.UpdatedAt!.Value);
        var updatedEntity2 = await Repository.GetById(insertResult2.Data!.Id, default);
        updatedEntity2.Data!.StringField.Should().Be("Updated2");
        updatedEntity2.Data!.UpdatedAt.Should().NotBeCloseTo(insertResult2.Data.UpdatedAt!.Value, TimeSpan.FromSeconds(0.8d));
        updatedEntity2.Data!.UpdatedAt.Should().BeAfter(insertResult2.Data.UpdatedAt!.Value);
    }

    [Fact]
    public async Task NoEntities_UpdateAtomicMultiple_ReturnsError()
    {
        var updateResult = await Repository.UpdateAtomic(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { Guid.NewGuid(), x => x.SetProperty(e => e.StringField, "Updated") },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<BulkUpdateOperationFailedError>();
    }

    [Fact]
    public async Task OneEntity_UpdateAtomicMultiple_ReturnsErrorAndEntityNotUpdated()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.UpdateAtomic(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.StringField, "Updated1") },
                { Guid.NewGuid(), x => x.SetProperty(e => e.StringField, "Updated2") },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<BulkUpdateOperationFailedError>();
        var updatedEntity = await Repository.GetById(insertResult.Data!.Id, default);
        updatedEntity.Data!.StringField.Should().Be(ValidEntity.StringField);
    }

    [Fact]
    public async Task ExistingEntity_UpdateAtomicMultipleCreatedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.UpdateAtomic(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.CreatedAt, DateTime.UtcNow.AddDays(-5)) },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateAtomicMultipleUpdatedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.UpdateAtomic(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.UpdatedAt, DateTime.UtcNow.AddDays(-5)) },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }

    [Fact]
    public async Task ExistingEntity_UpdateAtmoicMultipleDeletedAt_ReturnError()
    {
        var insertResult = await Repository.Insert(ValidEntity, default);
        insertResult.AssertNoError();

        var updateResult = await Repository.UpdateAtomic(
            new Dictionary<Guid, Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>>>()
            {
                { insertResult.Data!.Id, x => x.SetProperty(e => e.DeletedAt, DateTime.UtcNow.AddDays(-5)) },
            }, default);

        updateResult.HasError.Should().BeTrue();
        updateResult.Error.Should().BeOfType<InvalidUpdateEntityExpressionError>();
    }
}